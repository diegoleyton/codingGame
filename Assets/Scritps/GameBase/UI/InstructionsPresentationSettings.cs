using Flowbit.Engine.Definitions;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Flowbit.GameBase.UI
{
    /// <summary>
    /// ScriptableObject that maps each <see cref="InstructionType"/> to the UI prefab
    /// used to visually represent that instruction in the interface.
    /// 
    /// This allows designers to configure which UI prefab corresponds to each
    /// instruction without modifying code.
    /// </summary>
    [CreateAssetMenu(fileName = "InstructionsPresentationSettings", menuName = "CodingGame/Instructions Presentation Settings", order = 0)]
    public class InstructionsPresentationSettings : ScriptableObject
    {
        /// <summary>
        /// List of instruction-to-prefab mappings configured in the inspector.
        /// </summary>
        [SerializeField]
        private InstructionUi[] instructionsUi_;

        /// <summary>
        /// Runtime dictionary used for fast lookup of UI prefabs by instruction type.
        /// </summary>
        private Dictionary<InstructionType, GameObject> instructionsUiPrefabs_;

        /// <summary>
        /// Creates a UI instance for the specified instruction type.
        /// </summary>
        public GameObject CreateInstructionUi(InstructionType instructionType)
        {
            EnsureDictionaryInitialized();

            if (!instructionsUiPrefabs_.TryGetValue(instructionType, out var prefab))
            {
                throw new Exception($"No UI prefab registered for instruction type {instructionType}");
            }

            return Instantiate(prefab);
        }

        private void EnsureDictionaryInitialized()
        {
            if (instructionsUiPrefabs_ != null)
                return;

            instructionsUiPrefabs_ = new Dictionary<InstructionType, GameObject>();

            foreach (var entry in instructionsUi_)
            {
                if (instructionsUiPrefabs_.ContainsKey(entry.instructionType))
                {
                    throw new Exception(
                        $"Duplicate UI mapping found for instruction type {entry.instructionType}");
                }
                instructionsUiPrefabs_[entry.instructionType] = entry.instructionUiPrefab;
            }
        }
    }

    /// <summary>
    /// Defines the association between an <see cref="InstructionType"/> and the UI prefab
    /// that represents it in the interface.
    /// </summary>
    [Serializable]
    public struct InstructionUi
    {
        /// <summary>
        /// The instruction type.
        /// </summary>
        public InstructionType instructionType;

        /// <summary>
        /// Prefab used to display the instruction in the UI.
        /// </summary>
        public GameObject instructionUiPrefab;
    }
}