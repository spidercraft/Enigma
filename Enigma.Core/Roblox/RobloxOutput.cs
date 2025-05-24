using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Enigma.Core.Diagnostic;
using Enigma.Core.OpenVr.Model;
using Enigma.Core.Shim.Output;
using Enigma.Core.Shim.Window;
using InputSimulatorStandard.Native;
using RobloxFiles;
using IClipboard = Enigma.Core.Shim.Output.IClipboard;

namespace Enigma.Core.Roblox;

public class RobloxOutput
{
    /// <summary>
    /// Interval in milliseconds between sending heartbeat keys.
    /// </summary>
    public const int HeartbeatIntervalMilliseconds = 250;

    /// <summary>
    /// The last data that was sent to Roblox.
    /// </summary>
    public string LastData { get; private set; } = "";

    /// <summary>
    /// The last data that was requested to be sent to Roblox.
    /// This is not guaranteed to be the last data that was successfully sent.
    /// </summary>
    public string LastRequestedData { get; private set; } = "";
    
    /// <summary>
    /// Keyboard to send key inputs with.
    /// </summary>
    private readonly IKeyboard _keyboard;
    
    /// <summary>
    /// Clipboard to send data with.
    /// </summary>
    private readonly IClipboard _clipboard;

    /// <summary>
    /// Window state to check if Roblox is focused.
    /// </summary>
    private readonly BaseWindowState _windowState;

    /// <summary>
    /// Stopwatch to occasionally send a heartbeat key press.
    /// Roblox can't seem to consistently detect keys like F13 staying down.
    /// </summary>
    private readonly Stopwatch _heartbeatStopwatch = new Stopwatch();

    /// <summary>
    /// Creates a Roblox output.
    /// </summary>
    /// <param name="keyboard">Keyboard to use.</param>
    /// <param name="clipboard">Clipboard to use.</param>
    /// <param name="windowState">Window state to use.</param>
    public RobloxOutput(IKeyboard keyboard, IClipboard clipboard, BaseWindowState windowState)
    {
        this._keyboard = keyboard;
        this._clipboard = clipboard;
        this._windowState = windowState;
    }

    /// <summary>
    /// Pushes a string of text to the Roblox client.
    /// Due to the method of sending data, it is not guaranteed to work, even if true is returned since some messages
    /// will paste after the previous one instead of replacing it.
    /// </summary>
    /// <param name="data">String data to push.</param>
    /// <returns>Whether the data was pushed to the client.</returns>
    public async Task<bool> PushDataAsync(string data)
    {
        RobloxFile file = RobloxFile.Open(@"C:\Users\louki\AppData\Local\Bloxstrap\Versions\version-e00a4ca39fb04359\content\avatar\character.rbxm");

        // Make some changes...
        file.GetChildren().Last().SetAttribute("data", data);
        file.Save(@"C:\Users\louki\AppData\Local\Bloxstrap\Versions\version-e00a4ca39fb04359\content\avatar\character.rbxm");

        return true;
    }

    /// <summary>
    /// Pushes a list of tracker inputs to the Roblox client.
    /// </summary>
    /// <param name="trackerInputs">Tracker input data to push.</param>
    public async Task<bool> PushTrackersAsync(TrackerInputList trackerInputs)
    {
        return await this.PushDataAsync(trackerInputs.Serialize());
    }
}