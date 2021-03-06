﻿using System.Collections.Generic;
using System.Linq;
using HoloChessLibrary;
using UnityEngine;

namespace Assets.Scripts
{
    public class BoardSpace : MonoBehaviour
    {
        public Node Node { get; set; }
        public GameObject SelectionIndicatorPrefab;
        private GameObject _selectionIndicatorInstance;
        private Color OriginalColor { get; set; }
        private Color SelectionYellow { get; set; }

        private float? _tableScale;

        // Use this for initialization
        void Start()
        {
            SelectionYellow = new Color(1f, .89f, 0f, 1f);
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            OriginalColor = meshRenderer.material.color;
            Quaternion selectionIndicatorQuaternion =
                Quaternion.Euler(SelectionIndicatorPrefab.transform.rotation.eulerAngles.x,
                    SelectionIndicatorPrefab.transform.rotation.eulerAngles.y,
                    SelectionIndicatorPrefab.transform.rotation.eulerAngles.z);
            _selectionIndicatorInstance = Instantiate(SelectionIndicatorPrefab,
                new Vector3(transform.position.x, 0, transform.position.z),
                selectionIndicatorQuaternion);
            _selectionIndicatorInstance.SetActive(false);

            if (ClientGameState.Instance != null)
            {
                _tableScale = ClientGameState.Instance.Client.IsHost ? .1f : -.1f;
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (ClientGameState.Instance != null && _tableScale == null)
            {
                _tableScale = ClientGameState.Instance.Client.IsHost ? .1f : -.1f;
            }
        }

        void OnAvailableMonsters(IEnumerable<int> availableNodeIds)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

            meshRenderer.material.color = availableNodeIds.Contains(Node.Id) ? SelectionYellow : OriginalColor;
        }

        void OnAvailableAttacks(IEnumerable<int> availableNodeIds)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

            if (availableNodeIds.Contains(Node.Id))
            {
                meshRenderer.material.color = Color.red;
            }
            else if (meshRenderer.material.color == Color.red)
            {
                meshRenderer.material.color = OriginalColor;
            }
        }

        void OnAvailableMoves(IEnumerable<int> availableNodeIds)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

            if (availableNodeIds.Contains(Node.Id))
            {
                meshRenderer.material.color = Color.green;
            }
            else if (meshRenderer.material.color == Color.green)
            {
                meshRenderer.material.color = OriginalColor;
            }
        }

        void OnMonsterSelected(int nodeId)
        {
            if (_tableScale != null)
            {
                _selectionIndicatorInstance.transform.localPosition = new Vector3(Node.XPosition * _tableScale.Value + transform.position.x, Mathf.Abs(SelectionIndicatorPrefab.transform.position.y * 5 * _tableScale.Value) + transform.position.y, Node.YPosition * _tableScale.Value + transform.position.z);

                _selectionIndicatorInstance.SetActive(nodeId == Node.Id);

            }
        }

        void OnClearHighlighting()
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

            meshRenderer.material.color = OriginalColor;

            _selectionIndicatorInstance.SetActive(false);
        }

        void OnClearHighlightingWithSelection(Node selectedNode)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

            if (meshRenderer.material.color != SelectionYellow)
            {
                meshRenderer.material.color = OriginalColor;
            }

            if (selectedNode != null && selectedNode.Equals(Node))
            {
                _selectionIndicatorInstance.SetActive(true);
                meshRenderer.material.color = SelectionYellow;
            }
            else
            {
                _selectionIndicatorInstance.SetActive(false);
            }
        }

        void OnSelected(GameObject gameStateObject)
        {
            ClientGameState.Instance.SendMessage("OnSpaceSelected", Node.Id);
        }

        void OnPreviewEnter(GameObject gameStateObject)
        {
            ClientGameState.Instance.SendMessage("OnPreviewEnter", Node.Id);
        }

        void OnSpeechSelected(GameObject gameStateObject)
        {
            ClientGameState.Instance.SendMessage("OnSpaceSelected", Node.Id);
        }
    }
}