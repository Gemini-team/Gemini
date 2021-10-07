using System.Collections.Generic;
using UnityEngine;

public static class FerryInput {
	private const string JOYSTICK_PREFIX = "Joystick_", KEY_PREFIX = "Key";

	public static readonly Dictionary<string, KeyCode> defaultBindings = new Dictionary<string, KeyCode>() {
		{ "Left", KeyCode.A },
		{ "Right", KeyCode.D },
		{ "Back", KeyCode.S },
		{ "Forward", KeyCode.W },
		{ "YawLeft", KeyCode.Q },
		{ "YawRight", KeyCode.E },
		{ "Dock", KeyCode.F },
		{ "ManualTakeover", KeyCode.G }
	};

	private static readonly Dictionary<string, (string, string)> axes = new Dictionary<string, (string, string)>() {
		{ "Horizontal", ("Left", "Right") },
		{ "Throttle", ("Back", "Forward") },
		{ "Rudder", ("YawLeft", "YawRight") }
	};

	public static KeyCode GetBinding(string name) {
		System.Enum.TryParse(PlayerPrefs.GetString(KEY_PREFIX + name), out KeyCode kc);
		return kc == KeyCode.None ? defaultBindings[name] : kc;
	}

	public static void SetBinding(string name, KeyCode kc) {
		PlayerPrefs.SetString(KEY_PREFIX + name, kc.ToString());
	}

	public static bool GetButtonDown(string name) {
		if (Input.GetKeyDown(GetBinding(name))) return true;

		// Temporary implementation for joystick support
		try {
			return Input.GetButtonDown(JOYSTICK_PREFIX + name);
		} catch (System.ArgumentException) { }

		return false;
	}

	public static bool GetButton(string name) {
		if (Input.GetKey(GetBinding(name))) return true;

		// Temporary implementation for joystick support
		try {
			return Input.GetButton(JOYSTICK_PREFIX + name);
		}
		catch (System.ArgumentException) { }

		return false;
	}

	public static bool GetButtonUp(string name) {
		if (Input.GetKeyUp(GetBinding(name))) return true;

		// Temporary implementation for joystick support
		try {
			return Input.GetButtonUp(JOYSTICK_PREFIX + name);
		}
		catch (System.ArgumentException) { }

		return false;
	}

	public static float GetAxisRaw(string name) {
		var (negative, positive) = axes[name];

		int res = 0;
		if (GetButton(negative)) res -= 1;
		if (GetButton(positive)) res += 1;

		// Temporary implementation for joystick support
		return res == 0 ? Input.GetAxisRaw(JOYSTICK_PREFIX + name) : res;
	}
}
