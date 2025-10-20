using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Domain.Characters;

namespace LifeSim.Presentation.UI.Panels
{
    public class FamilyTreePanel : MonoBehaviour
    {
        // Internal class to represent the tree structure
        private class TreeNode
        {
            public Character CharacterData; 
            public CharacterNode NodeUI; 
            public TreeNode Parent;
            public List<TreeNode> Children = new List<TreeNode>();
        }

        [Header("Prefab & Container")]
        [SerializeField] private CharacterNode nodePrefab;
        [SerializeField] private Image linePrefab; // A simple 1x1 white pixel Image
        [SerializeField] private Transform container;
        [SerializeField] private Transform lineContainer;

        [Header("Layout Settings")]
        [SerializeField] private float horizontalSpacing = 160f;
        [SerializeField] private float verticalSpacing = 120f;

        private GameEngine _engine;
        private ILocalization _loc;
        private Dictionary<string, TreeNode> _treeNodes = new Dictionary<string, TreeNode>();

        public void Bind(GameEngine e, ILocalization l)
        {
            _engine = e;
            _loc = l;
            _engine.OnStateChanged += Refresh;
            Refresh(_engine.State);
        }

        void OnDestroy()
        {
            if (_engine != null) _engine.OnStateChanged -= Refresh;
        }

        public void Refresh(GameState state)
        {
            if (state == null) return;

            // Clear existing UI
            foreach (var node in _treeNodes.Values) { if(node.NodeUI) Destroy(node.NodeUI.gameObject); }
            _treeNodes.Clear();
            foreach (Transform child in lineContainer) { Destroy(child.gameObject); }

            // 1. Create all TreeNode wrappers
            foreach (var character in state.familyMembers.Values)
            {
                if (character.isAlive)
                {
                    _treeNodes[character.id] = new TreeNode { CharacterData = character };
                }
            }

            // 2. Build the parent-child relationships
            List<TreeNode> rootNodes = new List<TreeNode>();
            foreach (var treeNode in _treeNodes.Values)
            {
                if (string.IsNullOrEmpty(treeNode.CharacterData.parentId) || !_treeNodes.ContainsKey(treeNode.CharacterData.parentId))
                {
                    rootNodes.Add(treeNode);
                }
                else
                {
                    TreeNode parentNode = _treeNodes[treeNode.CharacterData.parentId];
                    parentNode.Children.Add(treeNode);
                    treeNode.Parent = parentNode;
                }
            }

            // 3. Instantiate UI and apply layout recursively from roots
            float yPos = 0;
            foreach (var rootNode in rootNodes.OrderBy(n => n.CharacterData.generation))
            {
                InstantiateAndPositionNode(rootNode, new Vector2(0, yPos));
                PositionChildrenRecursive(rootNode);
                // This simple yPos update assumes roots don't have complex overlaps
                // A more robust solution would calculate the total height of the subtree
                yPos -= 400; // Arbitrary large spacing for next root
            }

            // 4. Draw connector lines
            DrawAllConnectorLines();
        }

        private void InstantiateAndPositionNode(TreeNode treeNode, Vector2 position)
        {
            CharacterNode instance = Instantiate(nodePrefab, container);
            instance.transform.localPosition = position;
            instance.Setup(treeNode.CharacterData, _loc);
            treeNode.NodeUI = instance;
        }

        private void PositionChildrenRecursive(TreeNode parentTreeNode)
        {
            if (parentTreeNode.Children.Count == 0) return;

            float parentY = parentTreeNode.NodeUI.transform.localPosition.y;
            float childrenY = parentY - verticalSpacing;

            float totalWidth = (parentTreeNode.Children.Count - 1) * horizontalSpacing;
            float startX = parentTreeNode.NodeUI.transform.localPosition.x - totalWidth / 2f;

            for (int i = 0; i < parentTreeNode.Children.Count; i++)
            {
                TreeNode childNode = parentTreeNode.Children[i];
                float childX = startX + i * horizontalSpacing;
                InstantiateAndPositionNode(childNode, new Vector2(childX, childrenY));
                
                // Recurse
                PositionChildrenRecursive(childNode);
            }
        }

        private void DrawAllConnectorLines()
        {
            foreach (var treeNode in _treeNodes.Values)
            {
                if (treeNode.Parent != null && treeNode.Parent.NodeUI != null && treeNode.NodeUI != null)
                {
                    DrawConnectorLine(treeNode.Parent.NodeUI, treeNode.NodeUI);
                }
            }
        }

        private void DrawConnectorLine(CharacterNode from, CharacterNode to)
        {
            Vector2 fromPos = from.transform.localPosition;
            Vector2 toPos = to.transform.localPosition;

            Image line = Instantiate(linePrefab, lineContainer);
            RectTransform lineRect = line.rectTransform;

            Vector2 dir = (toPos - fromPos).normalized;
            float distance = Vector2.Distance(fromPos, toPos);

            lineRect.anchorMin = new Vector2(0, 0);
            lineRect.anchorMax = new Vector2(0, 0);
            lineRect.sizeDelta = new Vector2(distance, 2f); // 2px line thickness
            lineRect.localPosition = fromPos + dir * distance * 0.5f;
            lineRect.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        }
    }
}
