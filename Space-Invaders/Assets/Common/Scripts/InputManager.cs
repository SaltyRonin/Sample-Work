using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    public enum Axis
    {
        LX, LY, LT, // Left X, Left Y, Left Trigger
        RX, RY, RT, // Right X, Right Y, Right Trigger
        DX, DY      // DPad X, DPad Y
    }

    public enum Button
    {
        A, B, X, Y,
        L, R,
        Back, Start,
        LC, RC,       // Left-stick click, Right-stick click
        LXP, LXN, LYP, LYN, // Left X Positive/Negative, Left Y Positive/Negative
        RXP, RXN, RYP, RYN, // Right X Positive/Negative, Right Y Positive/Negative
        DXP, DXN, DYP, DYN, // DPad X Positive/Negative, DPad Y Positive/Negative
        LT, RT,
        Any,
        DoubleL, DoubleR
    }

    public struct ControllerState
    {
        public float LX, LY, LT, RX, RY, RT, DX, DY;

        public bool A, B, X, Y, L, R;
        public bool Back, Start, LC, RC;
        public bool LXP, LXN, LYP, LYN;
        public bool RXP, RXN, RYP, RYN;
        public bool DXP, DXN, DYP, DYN;
        public bool LTButton, RTButton;
        public bool Any
        {
            get
            {
                return A || B || X || Y || L || R || Back || Start || LC || RC || LTButton || RTButton;
            }
        }

        public bool ADown, BDown, XDown, YDown, LDown, RDown;
        public bool BackDown, StartDown, LCDown, RCDown;
        public bool LXPDown, LXNDown, LYPDown, LYNDown;
        public bool RXPDown, RXNDown, RYPDown, RYNDown;
        public bool DXPDown, DXNDown, DYPDown, DYNDown;
        public bool LTDown, RTDown;
        public bool AnyDown
        {
            get
            {
                return ADown || BDown || XDown || YDown || LDown || RDown || BackDown || StartDown || LCDown || RCDown || LTDown || RTDown;
            }
        }

        public bool DoubleL, DoubleR;
    }

    public struct MenuInputState
    {
        public bool Up, Down, Left, Right, Submit, Cancel, Start;
    }

    private static ControllerState controlState;
    private static ControllerState simState;
    private static MenuInputState menuState;
    private static bool mac;

    // Double press variables
    private static bool firstL, firstR;
    private static float timeL, timeR;

    // One-shot axis variables
    private static bool _lxp, _lxn, _lyp, _lyn;
    private static bool _rxp, _rxn, _ryp, _ryn;
    private static bool _dxp, _dxn, _dyp, _dyn;
    private static bool _rt, _lt;

    public static bool Simulated;

    public static void Init()
    {
        switch (SystemInfo.operatingSystemFamily)
        {
            case OperatingSystemFamily.MacOSX:
                mac = true;
                break;
            default:
                mac = false;
                break;
        }

        controlState = new ControllerState();
        simState = new ControllerState();
        menuState = new MenuInputState();
        Simulated = false;
    }

    public static void Clear()
    {
        controlState = new ControllerState();
        menuState = new MenuInputState();
    }

    public static void ClearSimulated()
    {
        simState = new ControllerState();
    }

    public static void Update()
    {
        menuState = new MenuInputState();

        if (Simulated)
        {
            simState.ADown = false;
            simState.BDown = false;
            simState.XDown = false;
            simState.YDown = false;
            simState.LDown = false;
            simState.RDown = false;
            simState.BackDown = false;
            simState.StartDown = false;
            simState.LCDown = false;
            simState.RCDown = false;
            simState.LXPDown = false;
            simState.LXNDown = false;
            simState.LYPDown = false;
            simState.LYNDown = false;
            simState.RXPDown = false;
            simState.RXNDown = false;
            simState.RYPDown = false;
            simState.RYNDown = false;
            simState.DXPDown = false;
            simState.DXNDown = false;
            simState.DYPDown = false;
            simState.DYNDown = false;
            simState.LTDown = false;
            simState.RTDown = false;
            simState.DoubleL = false;
            simState.DoubleR = false;
            return;
        }

        if (Settings.UseController)
        {
            controlState.LX = Input.GetAxisRaw("LeftX");
            controlState.LY = Input.GetAxisRaw("LeftY");

            if (mac)
            {
                controlState.RX = Input.GetAxisRaw("MAC_RightX");
                controlState.RY = Input.GetAxisRaw("MAC_RightY");
                controlState.LT = Input.GetAxisRaw("MAC_LeftTrigger");
                controlState.RT = Input.GetAxisRaw("MAC_RightTrigger");
                controlState.DX = Input.GetAxisRaw("MAC_DPadX");
                controlState.DY = Input.GetAxisRaw("MAC_DPadY");

                controlState.ADown = Input.GetKeyDown(KeyCode.Joystick1Button16);
                controlState.BDown = Input.GetKeyDown(KeyCode.Joystick1Button17);
                controlState.XDown = Input.GetKeyDown(KeyCode.Joystick1Button18);
                controlState.YDown = Input.GetKeyDown(KeyCode.Joystick1Button19);
                controlState.LDown = Input.GetKeyDown(KeyCode.Joystick1Button13);
                controlState.RDown = Input.GetKeyDown(KeyCode.Joystick1Button14);
                controlState.BackDown = Input.GetKeyDown(KeyCode.Joystick1Button10);
                controlState.StartDown = Input.GetKeyDown(KeyCode.Joystick1Button9);
                controlState.LCDown = Input.GetKeyDown(KeyCode.Joystick1Button11);
                controlState.RCDown = Input.GetKeyDown(KeyCode.Joystick1Button12);

                controlState.A = Input.GetKey(KeyCode.Joystick1Button16);
                controlState.B = Input.GetKey(KeyCode.Joystick1Button17);
                controlState.X = Input.GetKey(KeyCode.Joystick1Button18);
                controlState.Y = Input.GetKey(KeyCode.Joystick1Button19);
                controlState.L = Input.GetKey(KeyCode.Joystick1Button13);
                controlState.R = Input.GetKey(KeyCode.Joystick1Button14);
                controlState.Back = Input.GetKey(KeyCode.Joystick1Button10);
                controlState.Start = Input.GetKey(KeyCode.Joystick1Button9);
                controlState.LC = Input.GetKey(KeyCode.Joystick1Button11);
                controlState.RC = Input.GetKey(KeyCode.Joystick1Button12);
            }
            else
            {
                controlState.RX = Input.GetAxisRaw("WIN_RightX");
                controlState.RY = Input.GetAxisRaw("WIN_RightY");
                controlState.LT = Input.GetAxisRaw("WIN_LeftTrigger");
                controlState.RT = Input.GetAxisRaw("WIN_RightTrigger");
                controlState.DX = Input.GetAxisRaw("WIN_DPadX");
                controlState.DY = Input.GetAxisRaw("WIN_DPadY");

                controlState.ADown = Input.GetKeyDown(KeyCode.Joystick1Button0);
                controlState.BDown = Input.GetKeyDown(KeyCode.Joystick1Button1);
                controlState.XDown = Input.GetKeyDown(KeyCode.Joystick1Button2);
                controlState.YDown = Input.GetKeyDown(KeyCode.Joystick1Button3);
                controlState.LDown = Input.GetKeyDown(KeyCode.Joystick1Button4);
                controlState.RDown = Input.GetKeyDown(KeyCode.Joystick1Button5);
                controlState.BackDown = Input.GetKeyDown(KeyCode.Joystick1Button6);
                controlState.StartDown = Input.GetKeyDown(KeyCode.Joystick1Button7);
                controlState.LCDown = Input.GetKeyDown(KeyCode.Joystick1Button8);
                controlState.RCDown = Input.GetKeyDown(KeyCode.Joystick1Button9);

                controlState.A = Input.GetKey(KeyCode.Joystick1Button0);
                controlState.B = Input.GetKey(KeyCode.Joystick1Button1);
                controlState.X = Input.GetKey(KeyCode.Joystick1Button2);
                controlState.Y = Input.GetKey(KeyCode.Joystick1Button3);
                controlState.L = Input.GetKey(KeyCode.Joystick1Button4);
                controlState.R = Input.GetKey(KeyCode.Joystick1Button5);
                controlState.Back = Input.GetKey(KeyCode.Joystick1Button6);
                controlState.Start = Input.GetKey(KeyCode.Joystick1Button7);
                controlState.LC = Input.GetKey(KeyCode.Joystick1Button8);
                controlState.RC = Input.GetKey(KeyCode.Joystick1Button9);
            }
        }
        else
        {
            controlState.LX = Input.GetAxis("KEY_LeftX");
            controlState.LY = Input.GetAxis("KEY_LeftY");
            controlState.RX = Input.GetAxis("KEY_RightX");
            controlState.RY = Input.GetAxis("KEY_RightY");
            controlState.LT = Input.GetAxis("KEY_LeftTrigger");
            controlState.RT = Input.GetAxis("KEY_RightTrigger");
            controlState.DX = Input.GetAxis("KEY_DPadX");
            controlState.DY = Input.GetAxis("KEY_DPadY");

            controlState.ADown = Input.GetKeyDown(KeyCode.M);
            controlState.BDown = Input.GetKeyDown(KeyCode.Comma);
            controlState.XDown = Input.GetKeyDown(KeyCode.Period);
            controlState.YDown = Input.GetKeyDown(KeyCode.Slash);
            controlState.LDown = Input.GetKeyDown(KeyCode.U);
            controlState.RDown = Input.GetKeyDown(KeyCode.O);
            controlState.BackDown = Input.GetKeyDown(KeyCode.Backspace);
            controlState.StartDown = Input.GetKeyDown(KeyCode.Escape);
            controlState.LCDown = Input.GetKeyDown(KeyCode.C);
            controlState.RCDown = Input.GetKeyDown(KeyCode.N);

            controlState.A = Input.GetKey(KeyCode.M);
            controlState.B = Input.GetKey(KeyCode.Comma);
            controlState.X = Input.GetKey(KeyCode.Period);
            controlState.Y = Input.GetKey(KeyCode.Slash);
            controlState.L = Input.GetKey(KeyCode.U);
            controlState.R = Input.GetKey(KeyCode.O);
            controlState.Back = Input.GetKey(KeyCode.Backspace);
            controlState.Start = Input.GetKey(KeyCode.Escape);
            controlState.LC = Input.GetKey(KeyCode.C);
            controlState.RC = Input.GetKey(KeyCode.N);
        }

        controlState.DoubleL = false;
        controlState.DoubleR = false;

        if (controlState.LDown)
        {
            if (firstL)
            {
                if (Time.unscaledTime - timeL < 0.33f)
                {
                    controlState.DoubleL = true;
                }
                firstL = false;
            }
            else
            {
                firstL = true;
                timeL = Time.unscaledTime;
            }
        }
        else if (firstL && Time.unscaledTime - timeL >= 0.33f)
        {
            firstL = false;
        }

        if (controlState.RDown)
        {
            if (firstR)
            {
                if (Time.unscaledTime - timeR < 0.33f)
                {
                    controlState.DoubleR = true;
                }
                firstR = false;
            }
            else
            {
                firstR = true;
                timeR = Time.unscaledTime;
            }
        }
        else if (firstR && Time.unscaledTime - timeR >= 0.33f)
        {
            firstR = false;
        }

        // Left stick one-shot
        controlState.LXP = (controlState.LX >= 0.2f);
        controlState.LXPDown = (controlState.LXP && !_lxp);
        _lxp = controlState.LXP;
        controlState.LXN = (controlState.LX <= -0.2f);
        controlState.LXNDown = (controlState.LXN && !_lxn);
        _lxn = controlState.LXN;
        controlState.LYP = (controlState.LY >= 0.2f);
        controlState.LYPDown = (controlState.LYP && !_lyp);
        _lyp = controlState.LYP;
        controlState.LYN = (controlState.LY <= -0.2f);
        controlState.LYNDown = (controlState.LYN && !_lyn);
        _lyn = controlState.LYN;

        // Right stick one-shot
        controlState.RXP = (controlState.RX >= 0.2f);
        controlState.RXPDown = (controlState.RXP && !_rxp);
        _rxp = controlState.RXP;
        controlState.RXN = (controlState.RX <= -0.2f);
        controlState.RXNDown = (controlState.RXN && !_rxn);
        _rxn = controlState.RXN;
        controlState.RYP = (controlState.RY >= 0.2f);
        controlState.RYPDown = (controlState.RYP && !_ryp);
        _ryp = controlState.RYP;
        controlState.RYN = (controlState.RY <= -0.2f);
        controlState.RYNDown = (controlState.RYN && !_ryn);
        _ryn = controlState.RYN;

        // DPad one-shot
        controlState.DXP = (controlState.DX >= 0.2f);
        controlState.DXPDown = (controlState.DXP && !_dxp);
        _dxp = controlState.DXP;
        controlState.DXN = (controlState.DX <= -0.2f);
        controlState.DXNDown = (controlState.DXN && !_dxn);
        _dxn = controlState.DXN;
        controlState.DYP = (controlState.DY >= 0.2f);
        controlState.DYPDown = (controlState.DYP && !_dyp);
        _dyp = controlState.DYP;
        controlState.DYN = (controlState.DY <= -0.2f);
        controlState.DYNDown = (controlState.DYN && !_dyn);
        _dyn = controlState.DYN;

        // Triggers one-shot
        controlState.LTButton = (controlState.LT >= 0.2f);
        controlState.LTDown = (controlState.LTButton && !_lt);
        _lt = controlState.LTButton;
        controlState.RTButton = (controlState.RT >= 0.2f);
        controlState.RTDown = (controlState.RTButton && !_rt);
        _rt = controlState.RTButton;

        menuState.Submit = Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Return);
        menuState.Cancel = Input.GetKeyDown(KeyCode.Comma) || Input.GetKeyDown(KeyCode.Backspace);
        menuState.Start = Input.GetKeyDown(KeyCode.Escape);
        menuState.Up = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        menuState.Down = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
        menuState.Left = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        menuState.Right = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);

        if (Settings.UseController)
        {
            if (mac)
            {
                menuState.Submit |= Input.GetKeyDown(KeyCode.Joystick1Button16);
                menuState.Cancel |= Input.GetKeyDown(KeyCode.Joystick1Button17);
                menuState.Start |= Input.GetKeyDown(KeyCode.Joystick1Button9);
            }
            else
            {
                menuState.Submit |= Input.GetKeyDown(KeyCode.Joystick1Button0);
                menuState.Cancel |= Input.GetKeyDown(KeyCode.Joystick1Button1);
                menuState.Start |= Input.GetKeyDown(KeyCode.Joystick1Button7);
            }
            if (!menuState.Up) menuState.Up = controlState.LYPDown || controlState.DYPDown;
            if (!menuState.Down) menuState.Down = controlState.LYNDown || controlState.DYNDown;
            if (!menuState.Left) menuState.Left = controlState.LXNDown || controlState.DXNDown;
            if (!menuState.Right) menuState.Right = controlState.LXPDown || controlState.DXPDown;
        }
    }

    public static float GetAxis(Axis axis)
    {
        float val = 0f;
        if(Simulated)
        {
            switch (axis)
            {
                case Axis.LX: val = simState.LX; break;
                case Axis.LY: val = simState.LY; break;
                case Axis.LT: val = simState.LT; break;
                case Axis.RX: val = simState.RX; break;
                case Axis.RY: val = simState.RY; break;
                case Axis.RT: val = simState.RT; break;
                case Axis.DX: val = simState.DX; break;
                case Axis.DY: val = simState.DY; break;
            }
        }
        else
        {
            switch (axis)
            {
                case Axis.LX: val = controlState.LX; break;
                case Axis.LY: val = controlState.LY; break;
                case Axis.LT: val = controlState.LT; break;
                case Axis.RX: val = controlState.RX; break;
                case Axis.RY: val = controlState.RY; break;
                case Axis.RT: val = controlState.RT; break;
                case Axis.DX: val = controlState.DX; break;
                case Axis.DY: val = controlState.DY; break;
            }
        }
        return val;
    }

    public static bool GetButton(Button button)
    {
        bool val = false;
        if(Simulated)
        {
            switch (button)
            {
                case Button.A: val = simState.A; break;
                case Button.B: val = simState.B; break;
                case Button.X: val = simState.X; break;
                case Button.Y: val = simState.Y; break;
                case Button.L: val = simState.L; break;
                case Button.R: val = simState.R; break;
                case Button.Back: val = simState.Back; break;
                case Button.Start: val = simState.Start; break;
                case Button.LC: val = simState.LC; break;
                case Button.RC: val = simState.RC; break;
                case Button.LXP: val = simState.LXP; break;
                case Button.LXN: val = simState.LXN; break;
                case Button.LYP: val = simState.LYP; break;
                case Button.LYN: val = simState.LYN; break;
                case Button.RXP: val = simState.RXP; break;
                case Button.RXN: val = simState.RXN; break;
                case Button.RYP: val = simState.RYP; break;
                case Button.RYN: val = simState.RYN; break;
                case Button.DXP: val = simState.DXP; break;
                case Button.DXN: val = simState.DXN; break;
                case Button.DYP: val = simState.DYP; break;
                case Button.DYN: val = simState.DYN; break;
                case Button.LT: val = simState.LTButton; break;
                case Button.RT: val = simState.RTButton; break;
                case Button.Any: val = simState.Any; break;
                // No doubles (it's impossible to hold a double press)
            }
        }
        else
        {
            switch (button)
            {
                case Button.A: val = controlState.A; break;
                case Button.B: val = controlState.B; break;
                case Button.X: val = controlState.X; break;
                case Button.Y: val = controlState.Y; break;
                case Button.L: val = controlState.L; break;
                case Button.R: val = controlState.R; break;
                case Button.Back: val = controlState.Back; break;
                case Button.Start: val = controlState.Start; break;
                case Button.LC: val = controlState.LC; break;
                case Button.RC: val = controlState.RC; break;
                case Button.LXP: val = controlState.LXP; break;
                case Button.LXN: val = controlState.LXN; break;
                case Button.LYP: val = controlState.LYP; break;
                case Button.LYN: val = controlState.LYN; break;
                case Button.RXP: val = controlState.RXP; break;
                case Button.RXN: val = controlState.RXN; break;
                case Button.RYP: val = controlState.RYP; break;
                case Button.RYN: val = controlState.RYN; break;
                case Button.DXP: val = controlState.DXP; break;
                case Button.DXN: val = controlState.DXN; break;
                case Button.DYP: val = controlState.DYP; break;
                case Button.DYN: val = controlState.DYN; break;
                case Button.LT: val = controlState.LTButton; break;
                case Button.RT: val = controlState.RTButton; break;
                case Button.Any: val = controlState.Any; break;
                // No doubles (it's impossible to hold a double press)
            }
        }
        return val;
    }

    public static bool GetButtonDown(Button button)
    {
        bool val = false;
        if(Simulated)
        {
            switch (button)
            {
                case Button.A: val = simState.ADown; break;
                case Button.B: val = simState.BDown; break;
                case Button.X: val = simState.XDown; break;
                case Button.Y: val = simState.YDown; break;
                case Button.L: val = simState.LDown; break;
                case Button.R: val = simState.RDown; break;
                case Button.Back: val = simState.BackDown; break;
                case Button.Start: val = simState.StartDown; break;
                case Button.LC: val = simState.LCDown; break;
                case Button.RC: val = simState.RCDown; break;
                case Button.LXP: val = simState.LXPDown; break;
                case Button.LXN: val = simState.LXNDown; break;
                case Button.LYP: val = simState.LYPDown; break;
                case Button.LYN: val = simState.LYNDown; break;
                case Button.RXP: val = simState.RXPDown; break;
                case Button.RXN: val = simState.RXNDown; break;
                case Button.RYP: val = simState.RYPDown; break;
                case Button.RYN: val = simState.RYNDown; break;
                case Button.DXP: val = simState.DXPDown; break;
                case Button.DXN: val = simState.DXNDown; break;
                case Button.DYP: val = simState.DYPDown; break;
                case Button.DYN: val = simState.DYNDown; break;
                case Button.LT: val = simState.LTDown; break;
                case Button.RT: val = simState.RTDown; break;
                case Button.Any: val = simState.AnyDown; break;
                case Button.DoubleL: val = simState.DoubleL; break;
                case Button.DoubleR: val = simState.DoubleR; break;
            }
        }
        else
        {
            switch (button)
            {
                case Button.A: val = controlState.ADown; break;
                case Button.B: val = controlState.BDown; break;
                case Button.X: val = controlState.XDown; break;
                case Button.Y: val = controlState.YDown; break;
                case Button.L: val = controlState.LDown; break;
                case Button.R: val = controlState.RDown; break;
                case Button.Back: val = controlState.BackDown; break;
                case Button.Start: val = controlState.StartDown; break;
                case Button.LC: val = controlState.LCDown; break;
                case Button.RC: val = controlState.RCDown; break;
                case Button.LXP: val = controlState.LXPDown; break;
                case Button.LXN: val = controlState.LXNDown; break;
                case Button.LYP: val = controlState.LYPDown; break;
                case Button.LYN: val = controlState.LYNDown; break;
                case Button.RXP: val = controlState.RXPDown; break;
                case Button.RXN: val = controlState.RXNDown; break;
                case Button.RYP: val = controlState.RYPDown; break;
                case Button.RYN: val = controlState.RYNDown; break;
                case Button.DXP: val = controlState.DXPDown; break;
                case Button.DXN: val = controlState.DXNDown; break;
                case Button.DYP: val = controlState.DYPDown; break;
                case Button.DYN: val = controlState.DYNDown; break;
                case Button.LT: val = controlState.LTDown; break;
                case Button.RT: val = controlState.RTDown; break;
                case Button.Any: val = controlState.AnyDown; break;
                case Button.DoubleL: val = controlState.DoubleL; break;
                case Button.DoubleR: val = controlState.DoubleR; break;
            }
        }
        return val;
    }

    public static ControllerState GetState()
    {
        return (Simulated) ? simState: controlState;
    }

    public static MenuInputState GetMenuState()
    {
        return menuState;
    }

    public static void SimulateAxis(Axis axis, float value)
    {
        switch (axis)
        {
            case Axis.LX: simState.LX = value; break;
            case Axis.LY: simState.LY = value; break;
            case Axis.LT: simState.LT = value; break;
            case Axis.RX: simState.RX = value; break;
            case Axis.RY: simState.RY = value; break;
            case Axis.RT: simState.RT = value; break;
            case Axis.DX: simState.DX = value; break;
            case Axis.DY: simState.DY = value; break;
        }
    }

    public static void SimulateButtonPress(Button button)
    {
        switch (button)
        {
            case Button.A: simState.A = simState.ADown = true; break;
            case Button.B: simState.B = simState.BDown = true; break;
            case Button.X: simState.X = simState.XDown = true; break;
            case Button.Y: simState.Y = simState.YDown = true; break;
            case Button.L: simState.L = simState.LDown = true; break;
            case Button.R: simState.R = simState.RDown = true; break;
            case Button.Back: simState.Back = simState.BackDown = true; break;
            case Button.Start: simState.Start = simState.StartDown = true; break;
            case Button.LC: simState.LC = simState.LCDown = true; break;
            case Button.RC: simState.RC = simState.RCDown = true; break;
            case Button.LXP: simState.LXP = simState.LXPDown = true; break;
            case Button.LXN: simState.LXN = simState.LXNDown = true; break;
            case Button.LYP: simState.LYP = simState.LYPDown = true; break;
            case Button.LYN: simState.LYN = simState.LYNDown = true; break;
            case Button.RXP: simState.RXP = simState.RXPDown = true; break;
            case Button.RXN: simState.RXN = simState.RXNDown = true; break;
            case Button.RYP: simState.RYP = simState.RYPDown = true; break;
            case Button.RYN: simState.RYN = simState.RYNDown = true; break;
            case Button.DXP: simState.DXP = simState.DXPDown = true; break;
            case Button.DXN: simState.DXN = simState.DXNDown = true; break;
            case Button.DYP: simState.DYP = simState.DYPDown = true; break;
            case Button.DYN: simState.DYN = simState.DYNDown = true; break;
            case Button.LT: simState.LTButton = simState.LTDown = true; break;
            case Button.RT: simState.RTButton = simState.RTDown = true; break;
            case Button.DoubleL: simState.DoubleL = true; break;
            case Button.DoubleR: simState.DoubleR = true; break;
        }
    }

    public static void SimulateButtonRelease(Button button)
    {
        switch (button)
        {
            case Button.A: simState.A = false; break;
            case Button.B: simState.B = false; break;
            case Button.X: simState.X = false; break;
            case Button.Y: simState.Y = false; break;
            case Button.L: simState.L = false; break;
            case Button.R: simState.R = false; break;
            case Button.Back: simState.Back = false; break;
            case Button.Start: simState.Start = false; break;
            case Button.LC: simState.LC = false; break;
            case Button.RC: simState.RC = false; break;
            case Button.LXP: simState.LXP = false; break;
            case Button.LXN: simState.LXN = false; break;
            case Button.LYP: simState.LYP = false; break;
            case Button.LYN: simState.LYN = false; break;
            case Button.RXP: simState.RXP = false; break;
            case Button.RXN: simState.RXN = false; break;
            case Button.RYP: simState.RYP = false; break;
            case Button.RYN: simState.RYN = false; break;
            case Button.DXP: simState.DXP = false; break;
            case Button.DXN: simState.DXN = false; break;
            case Button.DYP: simState.DYP = false; break;
            case Button.DYN: simState.DYN = false; break;
            case Button.LT: simState.LTButton = false; break;
            case Button.RT: simState.RTButton = false; break;
        }
    }
}
