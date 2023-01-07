using System;
using UnityEngine;

namespace LD52
{
    public class GameManager : MonoBehaviour
    {
        private void Awake()
        {
            InputSystem.InputSystem.Init();
        }
    }
}
