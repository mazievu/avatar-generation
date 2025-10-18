using UnityEngine;
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
        [Header("Prefab & Container")]
        [SerializeField] private CharacterNode nodePrefab;
        [SerializeField] private Transform container;

        private GameEngine _engine;
        private ILocalization _loc;
        private Dictionary<string, CharacterNode> _nodes = new Dictionary<string, CharacterNode>();

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

            // TODO: Implement object pooling for performance
            foreach (var node in _nodes.Values) { Destroy(node.gameObject); }
            _nodes.Clear();

            foreach (var character in state.familyMembers.Values)
            {
                if (character.isAlive) // Only display living members for now
                {
                    CharacterNode instance = Instantiate(nodePrefab, container);
                    instance.Setup(character, _loc);
                    _nodes[character.id] = instance;
                }
            }

            ApplySimpleLayout();
            // TODO: Implement line drawing between nodes
        }

        private void ApplySimpleLayout()
        {
            // A very basic layout algorithm placeholder
            // Groups characters by generation and arranges them in rows.
            var groups = _nodes.Values.GroupBy(n => n.CharacterData.generation)
                                     .OrderBy(g => g.Key);

            float yPos = 0;
            foreach (var group in groups)
            {
                int countInGen = group.Count();
                float xStart = - (countInGen - 1) * 150 / 2f;
                int i = 0;
                foreach (var node in group)
                {
                    node.transform.localPosition = new Vector3(xStart + i * 150, yPos, 0);
                    i++;
                }
                yPos -= 200; // Move to the next row
            }

            // TODO: Implement Pan & Zoom controls
            // TODO: Implement a proper graph layout algorithm (e.g., Sugiyama)
        }
    }
}
