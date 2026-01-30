using ImGuiNET;
using Raylib_CSharp;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Windowing;
using Raylib_CSharp.Interact;
using rlImGui_cs;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using Common;
using Client;
using MessagePack;
using System.Runtime.CompilerServices;

class ClientMain
{
    static String addressBuf = new String("");
    static MessageHandler messageHandler;
    static ClientState clientState;
    static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    static void Main(string[] args)
    {
        Window.Init(600, 600, "Title");
        RenderTexture2D renderTarget = RenderTexture2D.Load(400, 400);
        rlImGui.Setup(true);
        bool dockspaceOpen = true;

        var lastMouse = Input.GetMousePosition();
        while (!Window.ShouldClose())
        {
            Graphics.BeginDrawing();

            var mouse = Input.GetMousePosition();

            // Raylib
            Graphics.ClearBackground(Color.SkyBlue);

            // ImGui
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            rlImGui.Begin();
            ImGuiViewportPtr v = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(v.WorkPos);
            ImGui.SetNextWindowSize(v.WorkSize);
            ImGui.SetNextWindowViewport(v.ID);
            ImGuiWindowFlags windowFlags = ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | 
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
            ImGui.Begin("Dockspace", windowFlags);

            if ((ImGui.GetIO().ConfigFlags & ImGuiConfigFlags.DockingEnable) == ImGuiConfigFlags.DockingEnable) {
                uint id = ImGui.GetID("MyDockspace");
                ImGui.DockSpace(id, new Vector2(0, 0), ImGuiDockNodeFlags.None);
            }


            Vector2 winPos = ImGui.GetWindowPos();
            Vector2 padding = ImGui.GetStyle().FramePadding;
            // Render to raylib render texture
            {
                Graphics.BeginTextureMode(renderTarget);
                Graphics.ClearBackground(Color.SkyBlue);
                Graphics.DrawText("Basic Window!", 10, 10, 10, Color.White);
                Graphics.DrawCircle((int)(mouse.X - winPos.X - padding.X), (int)(mouse.Y - winPos.Y - padding.Y * 4), 5, Color.White);

                if (clientState != null)
                {
                    foreach (var item in clientState.clients)
                    {
                        Graphics.DrawCircle((int)(item.Value.mouseX - winPos.X - padding.X), (int)(item.Value.mouseY - winPos.Y - padding.Y * 4), 5, Color.White);
                    }
                }
                Graphics.EndTextureMode();
            }

            // Render the render texture in imgui
            {
                ImGui.Begin("Render Target");
                rlImGui.ImageRenderTexture(renderTarget);
                ImGui.End();
            }

            // Render config/information panel in imgui
            {
                ImGui.Begin("Config Panel");
                if (ImGui.InputText("IP Address", ref addressBuf, 15, ImGuiInputTextFlags.EnterReturnsTrue))
                {
                    try
                    {
                        TcpClient client = new TcpClient();
                        IPAddress addr = IPAddress.Parse(addressBuf);
                        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(addressBuf), 33);
                        client.Connect(endPoint);
                        cancellationTokenSource = new CancellationTokenSource();
                        clientState = new ClientState(ref client);
                        messageHandler = new MessageHandler(ref clientState);

                        Task.Run(async () =>
                        {
                            await messageHandler.Run(cancellationTokenSource.Token);
                        });
                    }
                    catch (ArgumentNullException e)
                    {
                        Console.WriteLine("ArgumentNullException caught!!!");
                        Console.WriteLine("Source : " + e.Source);
                        Console.WriteLine("Message : " + e.Message);
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine("FormatException caught!!!");
                        Console.WriteLine("Source : " + e.Source);
                        Console.WriteLine("Message : " + e.Message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception caught!!!");
                        Console.WriteLine("Source : " + e.Source);
                        Console.WriteLine("Message : " + e.Message);
                    }
                }
                ImGui.Text($"WinPos: {winPos}");
                ImGui.Text($"MousePos: {mouse}");
                ImGui.Text($"RelPos: {mouse - winPos}");
                if (clientState != null && clientState.guid != null)
                {
                    ImGui.Text($"Guid: {clientState.guid}");
                }
                else
                {
                    ImGui.Text("Guid not set");
                }
                if (clientState != null && clientState.client != null && clientState.client.Connected)
                {
                    if (ImGui.Button("Disconnect"))
                    {
                        clientState.Disconnect();
                        cancellationTokenSource.Cancel();
                    }

                    if (ImGui.Button("Send message"))
                    {
                        clientState.SendMessage(new Test("hi"));
                    }
                    if (mouse != lastMouse)
                    {
                        clientState.SendMessage(new MouseMove((int)mouse.X, (int)mouse.Y));
                    }
                }
                ImGui.End();
            }

            ImGui.End(); // Dockspace
            rlImGui.End();

            Graphics.EndDrawing();
            lastMouse = mouse;
        }

        rlImGui.Shutdown();
        Window.Close();
        if (clientState != null && clientState.client != null && clientState.client.Connected)
            clientState.client.Close();
    }
}