using System;
using System.Runtime.InteropServices;

namespace PsConsoleLauncher.ViewFunctions
{
	public static class ControllerHelper
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct XINPUT_STATE
		{
			public uint dwPacketNumber;
			public XINPUT_GAMEPAD Gamepad;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct XINPUT_GAMEPAD
		{
			public ushort wButtons;
			public byte bLeftTrigger;
			public byte bRightTrigger;
			public short sThumbLX;
			public short sThumbLY;
			public short sThumbRX;
			public short sThumbRY;
		}

		private const int ERROR_SUCCESS = 0;
		private const ushort XINPUT_GAMEPAD_DPAD_LEFT = 0x0001;
		private const ushort XINPUT_GAMEPAD_DPAD_RIGHT = 0x0002;
		private const ushort XINPUT_GAMEPAD_A = 0x1000;

		[DllImport("xinput1_4.dll")]
		private static extern int XInputGetState(int dwUserIndex, out XINPUT_STATE pState);

		public struct ReadState
		{
			public bool Connected;
			public bool DPadLeft;
			public bool DPadRight;
			public bool ButtonA;
			public float LeftThumbX;
		}

		public static ReadState GetState(int index)
		{
			var outState = new ReadState();
			try
			{
				if (XInputGetState(index, out XINPUT_STATE state) == ERROR_SUCCESS)
				{
					outState.Connected = true;
					var buttons = state.Gamepad.wButtons;
					outState.DPadLeft = (buttons & XINPUT_GAMEPAD_DPAD_LEFT) != 0;
					outState.DPadRight = (buttons & XINPUT_GAMEPAD_DPAD_RIGHT) != 0;
					outState.ButtonA = (buttons & XINPUT_GAMEPAD_A) != 0;
					outState.LeftThumbX = state.Gamepad.sThumbLX / 32767f;
				}
			}
			catch
			{
				// If xinput dll not present the call will throw; return disconnected
				outState.Connected = false;
			}
			return outState;
		}
	}
}
