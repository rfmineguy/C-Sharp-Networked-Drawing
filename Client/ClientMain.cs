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
using Common;
using Client;

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
            Graphics.BeginTextureMode(renderTarget);
            Graphics.ClearBackground(Color.SkyBlue);
            Graphics.DrawText("Basic Window!", 10, 10, 20, Color.White);
            Graphics.EndTextureMode();

            // ImGui
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            rlImGui.Begin();
            ImGuiViewportPtr v = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(v.WorkPos);
            ImGui.SetNextWindowSize(v.WorkSize);
            ImGui.SetNextWindowViewport(v.ID);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGuiWindowFlags windowFlags = ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | 
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
            ImGui.Begin("Dockspace", windowFlags);
            ImGui.PopStyleVar(2);

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

            ImGui.Begin("Render Target");
            rlImGui.ImageRenderTexture(renderTarget);
            ImGui.End();

            ImGui.Begin("Config Panel");
            ImGui.End();

            ImGui.End(); // Dockspace
            rlImGui.End();

            Graphics.EndDrawing();
        }

        rlImGui.Shutdown();
        Window.Close();
        /*IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 33);
        using var client = new TcpClient();

        AppDomain.CurrentDomain.ProcessExit += (s, e) => { Console.WriteLine("Exitting..."); client.GetStream().Close(); client.Dispose(); };
        try
        {
            client.Connect(endPoint);
            while (client.Connected)
            {
                Console.Write("H");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }*/

        //client.GetStream().Close();
        //client.Close();
    }
}