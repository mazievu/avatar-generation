using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using LifeSim.Core.Engine;
using LifeSim.Core.Services;
using LifeSim.Core.Domain.Characters;
using LifeSim.Core.Data;

namespace LifeSim.Presentation.UI
{
    public class EmployeeSelectionModal : ModalBase
    {
        [Header("UI References")]
        [SerializeField] private CandidateSlotUI candidatePrefab;
        [SerializeField] private Transform container;
        [SerializeField] private Button closeButton;

        private GameEngine _engine;
        private ILocalization _loc;
        private string _instanceId;
        private int _slotIndex;

        void Awake()
        {
            closeButton?.onClick.AddListener(Close);
        }

        public void Bind(GameEngine engine, ILocalization loc)
        {
            _engine = engine;
            _loc = loc;
        }

        public void Show(string instanceId, int slotIndex)
        {
            _instanceId = instanceId;
            _slotIndex = slotIndex;
            RefreshCandidates();
            Open();
        }

        public override void Open()
        {
            gameObject.SetActive(true);
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }

        private void RefreshCandidates()
        {
            foreach (Transform child in container) { Destroy(child.gameObject); }

            var allCharacters = _engine.State.familyMembers.Values;

            var eligibleCandidates = allCharacters.Where(c => {
                int age = c.ageDays / Constants.DaysPerYear;
                return c.isAlive && 
                       age > 18 && 
                       age < 60 && 
                       c.status == CharacterStatus.Idle;
            }).OrderByDescending(c => c.stats.skill).ToList();

            foreach (var candidate in eligibleCandidates)
            {
                var slot = Instantiate(candidatePrefab, container);
                slot.Setup(candidate, _loc, OnCandidateSelected);
            }
        }

        private void OnCandidateSelected(string characterId)
        {
            _engine.AssignEmployeeToBusiness(_instanceId, _slotIndex, characterId);
            Close();
        }
    }
}
