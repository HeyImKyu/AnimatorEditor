using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class AnimatorEditor : UnityEditor.EditorWindow
{
    [UnityEditor.MenuItem("Tools/Kyu/Animator Editor")]
    public static void ShowWindow()
    {
        GetWindow<AnimatorEditor>("Animator Editor");
    }

    // ---- bool ----
    public Vector2 scrollPosition { get; set; }

    public AnimatorController animatorController { get; set; }
    public GameObject gameobject { get; set; }
    public Motion animationOn { get; set; }
    public Motion animationOff { get; set; }
    public string layerName { get; set; }
    public string onTransitionParam { get; set; }
    public string offTransitionParam { get; set; }
    public bool onTransitionBool { get; set; }
    public bool offTransitionBool { get; set; }
    public bool showOnOff { get; set; }

    // ---- int ----
    public bool showIntSwitcher { get; set; }

    public int numOfIntAnimations { get; set; }
    public List<Motion> intMotions { get; set; } = new List<Motion>();
    public string intTransitionParam { get; set; }

    // ---- hue ----
    public bool showHueShifter { get; set; }

    public string hueTransitionParam { get; set; }
    public Motion animation0 { get; set; }
    public Motion animation1 { get; set; }

    // ---- flip ----
    public bool showAnimationFlipper { get; set; }
    public int numOfFlipAnimations { get; set; }
    public List<AnimationClip> animationsToFlip { get; set; } = new List<AnimationClip>();

    private void OnGUI()
    {
        var labelOptions = new GUILayoutOption[]
             {
             GUILayout.ExpandWidth(false),
             GUILayout.MaxWidth(13)
             };
        char upArrow = '\u25B2';
        char downArrow = '\u25BC';

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.BeginHorizontal();
        animatorController = (AnimatorController)EditorGUILayout.ObjectField("Animator Controller", animatorController, typeof(AnimatorController), true);
        if (animatorController == null)
            gameobject = (GameObject)EditorGUILayout.ObjectField("GameObject", gameobject, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();

        if (gameobject != null)
        {
            var runtimeAC = gameobject.GetComponent<Animator>().runtimeAnimatorController;
            animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GetAssetPath(runtimeAC));
        }

        layerName = EditorGUILayout.TextField("New Layer Name", layerName);

        EditorGUILayout.Space();
        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        rect.height = 1;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        EditorGUILayout.Space();

        #region onOffToggle

        showOnOff = EditorGUILayout.Foldout(showOnOff, "On-Off Toggle");
        if (showOnOff)
        {
            animationOff = (Motion)EditorGUILayout.ObjectField("Off Animation", animationOff, typeof(Motion), true);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            onTransitionBool = EditorGUILayout.ToggleLeft("", onTransitionBool, labelOptions);
            onTransitionParam = EditorGUILayout.TextField("", onTransitionParam);

            EditorGUILayout.LabelField(downArrow.ToString(), labelOptions);
            EditorGUILayout.LabelField(upArrow.ToString(), labelOptions);

            offTransitionParam = EditorGUILayout.TextField("", offTransitionParam);
            offTransitionBool = EditorGUILayout.ToggleLeft("", offTransitionBool, labelOptions);

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            animationOn = (Motion)EditorGUILayout.ObjectField("On Animation", animationOn, typeof(Motion), true);
            EditorGUILayout.Space();

            if (GUILayout.Button("Add On-Off-Layer"))
            {
                AddOnOffToggle();
            }
        }

        #endregion onOffToggle

        #region IntSwitcher

        showIntSwitcher = EditorGUILayout.Foldout(showIntSwitcher, "Int-Switcher");
        if (showIntSwitcher)
        {
            intTransitionParam = EditorGUILayout.TextField("Transition Paramter", intTransitionParam);
            EditorGUILayout.Space();

            numOfIntAnimations = EditorGUILayout.IntField("Number of Animations", numOfIntAnimations);

            EditorGUILayout.Space();

            for (int i = 0; i < numOfIntAnimations; i++)
            {
                intMotions.Add(null);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                intMotions[i] = (Motion)EditorGUILayout.ObjectField("Animation " + i, intMotions[i], typeof(Motion), true);
                EditorGUILayout.EndHorizontal();
            }
            if (numOfIntAnimations != 0)
                while (numOfIntAnimations < intMotions.Count)
                {
                    intMotions.RemoveAt(intMotions.Count - 1);
                }

            EditorGUILayout.Space();

            if (GUILayout.Button("Add Int-Switch Layer"))
            {
                AddIntToggle();
            }
        }

        #endregion IntSwitcher

        #region HueShifter

        showHueShifter = EditorGUILayout.Foldout(showHueShifter, "Hue-Shift Layer");
        if (showHueShifter)
        {
            hueTransitionParam = EditorGUILayout.TextField("Transition Paramter", hueTransitionParam);
            EditorGUILayout.Space();

            animation0 = (Motion)EditorGUILayout.ObjectField("Animation 0", animation0, typeof(Motion), true);
            EditorGUILayout.Space();
            animation1 = (Motion)EditorGUILayout.ObjectField("Animation 1", animation1, typeof(Motion), true);

            EditorGUILayout.Space();
            if (GUILayout.Button("Create Hue-Shift Layer"))
            {
                AddHueShift();
            }
        }

        #endregion HueShifter

        #region FlipAnimations
        showAnimationFlipper = EditorGUILayout.Foldout(showAnimationFlipper, "Animation Flipper");
        if (showAnimationFlipper)
        {
            numOfFlipAnimations = EditorGUILayout.IntField("Number of Animations", numOfFlipAnimations);

            EditorGUILayout.Space();

            for (int i = 0; i < numOfFlipAnimations; i++)
            {
                animationsToFlip.Add(null);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                animationsToFlip[i] = (AnimationClip)EditorGUILayout.ObjectField("Animation " + i, animationsToFlip[i], typeof(AnimationClip), true);
                EditorGUILayout.EndHorizontal();
            }
            if (numOfFlipAnimations != 0)
                while (numOfFlipAnimations < animationsToFlip.Count)
                {
                    animationsToFlip.RemoveAt(animationsToFlip.Count - 1);
                }

            EditorGUILayout.Space();
            if (GUILayout.Button("Flip Animations"))
            {
                FlipAnimations();
            }
        }
        #endregion FlipAnimations

        EditorGUILayout.EndScrollView();

        void AddOnOffToggle()
        {
            //param creation
            bool onTransitionExists = false;
            bool offTransitionExists = false;

            foreach (var param in animatorController.parameters)
            {
                if (param.name == offTransitionParam && param.type == AnimatorControllerParameterType.Bool)
                    offTransitionExists = true;
                if (param.name == onTransitionParam && param.type == AnimatorControllerParameterType.Bool)
                    onTransitionExists = true;
            }

            if (offTransitionParam == onTransitionParam && !offTransitionExists && !onTransitionExists)
                animatorController.AddParameter(onTransitionParam, AnimatorControllerParameterType.Bool);
            else if (!onTransitionExists || !offTransitionExists)
            {
                if (!onTransitionExists)
                    animatorController.AddParameter(onTransitionParam, AnimatorControllerParameterType.Bool);
                if (!offTransitionExists)
                    animatorController.AddParameter(offTransitionParam, AnimatorControllerParameterType.Bool);
            }

            //new Layer
            var layer = new AnimatorControllerLayer();
            layer.stateMachine = new AnimatorStateMachine();

            layer.name = layerName;
            layer.defaultWeight = 1;

            AnimatorState stateOn = layer.stateMachine.AddState("On", new Vector3(250, 200, 0));
            AnimatorState stateOff = layer.stateMachine.AddState("Off", new Vector3(250, 120, 0));

            layer.stateMachine.defaultState = stateOff;
            var offToOn = stateOff.AddTransition(stateOn);
            var onToOff = stateOn.AddTransition(stateOff);

            if (onTransitionBool)
                offToOn.AddCondition(AnimatorConditionMode.If, 0, onTransitionParam);
            else
                offToOn.AddCondition(AnimatorConditionMode.IfNot, 0, onTransitionParam);

            if (offTransitionBool)
                onToOff.AddCondition(AnimatorConditionMode.If, 0, offTransitionParam);
            else
                onToOff.AddCondition(AnimatorConditionMode.IfNot, 0, offTransitionParam);

            stateOff.motion = animationOff;
            stateOn.motion = animationOn;

            animatorController.AddLayer(layer);
        }

        void AddIntToggle()
        {
            //param creation
            bool paramExists = false;

            foreach (var param in animatorController.parameters)
            {
                if (param.name == intTransitionParam && param.type == AnimatorControllerParameterType.Int)
                    paramExists = true;
            }

            if (!paramExists)
                animatorController.AddParameter(intTransitionParam, AnimatorControllerParameterType.Int);

            //new Layer
            var layer = new AnimatorControllerLayer();
            layer.stateMachine = new AnimatorStateMachine();

            layer.name = layerName;
            layer.defaultWeight = 1;

            List<AnimatorState> states = new List<AnimatorState>();

            for (int i = 0; i < intMotions.Count; i++)
            {
                Motion Motion = (Motion)intMotions[i];
                if (Motion == null)
                    continue;

                states.Add(layer.stateMachine.AddState(Motion.name, new Vector3(500, 80 * i)));
                states[states.Count - 1].motion = Motion;
            }
            states.Add(layer.stateMachine.AddState("Default", new Vector3(250, 120, 0)));
            layer.stateMachine.defaultState = states[states.Count - 1];

            for (int i = 0; i < states.Count - 1; i++)
            {
                AnimatorState state = states[i];

                states[states.Count - 1].AddTransition(state).AddCondition(AnimatorConditionMode.Equals, i, intTransitionParam);
                state.AddTransition(states[states.Count - 1]).AddCondition(AnimatorConditionMode.NotEqual, i, intTransitionParam);
            }

            animatorController.AddLayer(layer);
        }

        void AddHueShift()
        {
            //param creation
            bool paramExists = false;

            foreach (var param in animatorController.parameters)
            {
                if (param.name == hueTransitionParam && param.type == AnimatorControllerParameterType.Float)
                    paramExists = true;
            }

            if (!paramExists)
                animatorController.AddParameter(hueTransitionParam, AnimatorControllerParameterType.Float);

            //new Layer
            var layer = new AnimatorControllerLayer();
            layer.stateMachine = new AnimatorStateMachine();

            layer.name = layerName;
            layer.defaultWeight = 1;

            AnimatorState blendTreeState = layer.stateMachine.AddState("Hue Shift", new Vector3(250, 120, 0));

            BlendTree blendTree = new BlendTree();
            blendTree.useAutomaticThresholds = true;
            blendTree.blendParameter = hueTransitionParam;
            blendTree.name = "Hue Shift";
            blendTree.AddChild(animation0);
            blendTree.AddChild(animation1);

            blendTreeState.motion = blendTree;

            layer.stateMachine.defaultState = blendTreeState;

            animatorController.AddLayer(layer);
        }
    }

    private void FlipAnimations()
    {
        foreach (var anim in animationsToFlip)
        {
            var path = AssetDatabase.GetAssetPath(anim);
            var clip = new AnimationClip();

            var settings = AnimationUtility.GetAnimationClipSettings(anim);
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            var allCurves = AnimationUtility.GetAllCurves(anim);
            EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(anim);
            foreach (var binding in bindings)
            {
                var animCurve = AnimationUtility.GetEditorCurve(anim, binding);
                var newCurve = new AnimationCurve();
                for (int i = 0; i < animCurve.length; i++)
                {
                    Keyframe curve = animCurve[i];
                    if (curve.value == 0f)
                        curve.value = 1f;
                    else if (curve.value == 1f) // thank you @Skadihehe ;-;
                        curve.value = 0f;

                    newCurve.AddKey(curve);
                }

                AnimationUtility.SetEditorCurve(clip, binding, newCurve);
            }

            var name = anim.name.EndsWith("0") ? anim.name.Substring(0, anim.name.Length - 1) + "1" : anim.name + "1";

            AssetDatabase.CreateAsset(clip, path.Substring(0, path.LastIndexOf('/')) + "/" + name + ".anim");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            clip.frameRate = anim.frameRate;
        }
    }
}