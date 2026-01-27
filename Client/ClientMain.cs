using ImGuiNET;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Windowing;
using rlImGui_cs;
using System.Numerics;

class ClientMain
{
    static void Main(string[] args)
    {
        Window.Init(600, 600, "Title");
        RenderTexture2D renderTarget = RenderTexture2D.Load(400, 400);
        rlImGui.Setup(true);
        bool dockspaceOpen = true;

        while (!Window.ShouldClose())
        {
            Graphics.BeginDrawing();

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
            ImGui.Begin("Render Target");
            rlImGui.ImageRenderTexture(renderTarget);
            ImGui.End();

            ImGui.Begin("Config Panel");
            ImGui.End();

            ImGui.End();
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