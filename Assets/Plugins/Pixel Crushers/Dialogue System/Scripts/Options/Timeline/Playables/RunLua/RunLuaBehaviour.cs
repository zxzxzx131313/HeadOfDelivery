// Recompile at 9/17/2023 12:01:08 AM

#if USE_TIMELINE
#if UNITY_2017_1_OR_NEWER
// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Playables;
using System;

namespace PixelCrushers.DialogueSystem
{

    [Serializable]
    public class RunLuaBehaviour : PlayableBehaviour
    {

        [Tooltip("Run this Lua code.")]
        [TextArea(5, 5)]
        public string luaCode;

    }
}
#endif
#endif
