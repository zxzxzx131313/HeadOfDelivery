
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
#endif
[Preserve]
[DisplayStringFormat("{up}/{left}/{down}/{right}")]
public class WASDComposite : InputBindingComposite<Vector2>
{

    // NOTE: This is a modified copy of Vector2Composite

    [InputControl(layout = "Button")]
    public int up = 0;
    [InputControl(layout = "Button")]
    public int down = 0;
    [InputControl(layout = "Button")]
    public int left = 0;
    [InputControl(layout = "Button")]
    public int right = 0;

    private bool upPressedLastFrame;
    private bool downPressedLastFrame;
    private bool leftPressedLastFrame;
    private bool rightPressedLastFrame;
    private float upPressTimestamp;
    private float downPressTimestamp;
    private float leftPressTimestamp;
    private float rightPressTimestamp;
    private float upReleaseTimestamp;
    private float downReleaseTimestamp;
    private float leftReleaseTimestamp;
    private float rightReleaseTimestamp;

    public override Vector2 ReadValue(ref InputBindingCompositeContext context)
    {

        var upPressed = context.ReadValueAsButton(up);
        var downPressed = context.ReadValueAsButton(down);
        var leftPressed = context.ReadValueAsButton(left);
        var rightPressed = context.ReadValueAsButton(right);

        if (!upPressed && upPressedLastFrame) upReleaseTimestamp = Time.time;
        if (!downPressed && downPressedLastFrame) downReleaseTimestamp = Time.time;
        if (!leftPressed && leftPressedLastFrame) leftReleaseTimestamp = Time.time;
        if (!rightPressed && rightPressedLastFrame) rightReleaseTimestamp = Time.time;

        if (upPressed && !upPressedLastFrame) upPressTimestamp = Time.time;
        if (downPressed && !downPressedLastFrame) downPressTimestamp = Time.time;
        if (leftPressed && !leftPressedLastFrame) leftPressTimestamp = Time.time;
        if (rightPressed && !rightPressedLastFrame) rightPressTimestamp = Time.time;

        // among all the events, the pressed ones must be the newest to be triggered
        float[] timestamps = { upPressTimestamp, downPressTimestamp, leftPressTimestamp, rightPressTimestamp, upReleaseTimestamp, downReleaseTimestamp, rightReleaseTimestamp, leftReleaseTimestamp };
        float x = (leftPressed, rightPressed) switch
        {
            (false, false) => 0f,
            (true, false) => leftPressTimestamp == Mathf.Max(timestamps) ? -1f : 0f,
            (false, true) => rightPressTimestamp == Mathf.Max(timestamps) ? 1f : 0f,
            (true, true) when rightPressTimestamp > leftPressTimestamp => 1f,
            (true, true) when rightPressTimestamp < leftPressTimestamp => -1f,
            (true, true) => 0f
        };

        float y = (downPressed, upPressed) switch
        {
            (false, false) => 0f,
            (true, false) => downPressTimestamp == Mathf.Max(timestamps) ? -1f : 0f,
            (false, true) => upPressTimestamp == Mathf.Max(timestamps) ? 1f : 0f,
            (true, true) when upPressTimestamp > downPressTimestamp => 1f,
            (true, true) when upPressTimestamp < downPressTimestamp => -1f,
            (true, true) => 0f
        };

        if (x != 0f && y != 0f)
        {
            float verticalTimestamp, horzontalTimestamp;
            verticalTimestamp = upPressTimestamp < downPressTimestamp ? downPressTimestamp : upPressTimestamp;
            horzontalTimestamp = leftPressTimestamp < rightPressTimestamp ? rightPressTimestamp : leftPressTimestamp;
            if (verticalTimestamp < horzontalTimestamp) y = 0f;
            else x = 0f;
        }

        upPressedLastFrame = upPressed;
        downPressedLastFrame = downPressed;
        leftPressedLastFrame = leftPressed;
        rightPressedLastFrame = rightPressed;

        return new Vector2(x, y);
    }

    public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
    {
        var value = ReadValue(ref context);
        return value.magnitude;
    }

#if UNITY_EDITOR
    static WASDComposite()
    {
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Initialize()
    {
        InputSystem.RegisterBindingComposite<WASDComposite>();
    }
}