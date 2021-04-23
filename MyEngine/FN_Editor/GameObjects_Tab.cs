using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using System.Linq;

namespace MyEngine.FN_Editor
{
    enum Operation { Delete, Create, Rename};

    class GameObjects_Tab : GameObjectComponent
    {
        public static GameObject WhoIsSelected = null;

        private string NameBuffer = "";
        private HashSet<GameObject> SelectedGOs;
        private GameObject[] GOs_Clipboard = null;
        private bool IsCopy = true;
        private LinkedList<KeyValuePair<object, Operation>> Undo_Buffer;
        private LinkedList<KeyValuePair<object, Operation>> Redo_Buffer;
        private int BufferLimit = 200; //200 Items

        public override void Start()
        {
            WhoIsSelected = null;
            SelectedGOs = new HashSet<GameObject>();
            Undo_Buffer = new LinkedList<KeyValuePair<object, Operation>>();
            Redo_Buffer = new LinkedList<KeyValuePair<object, Operation>>();
            LinkedList<int> T = new LinkedList<int>();
        }

        public override void DrawUI()
        {
            //Scene Tab
            ImGui.Begin(SceneManager.ActiveScene.Name);

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && ImGui.IsWindowHovered(ImGuiHoveredFlags.RootWindow) && !ImGui.IsAnyItemHovered())
            {
                WhoIsSelected = null;
                SelectedGOs.Clear();
            }

            ImGui.Indent();

            foreach (GameObject GO in SceneManager.ActiveScene.GameObjects)
                if (!GO.IsEditor && GO.Parent == null && !GO.ShouldBeDeleted)
                    TreeRecursive(GO, true);

            //Deleting a GameObject
            if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.Delete)) && ImGui.IsWindowFocused())
            {
                if (SelectedGOs.Count != 0)
                {
                    AddToACircularBuffer(Undo_Buffer, new KeyValuePair<object, Operation>(SelectedGOs.ToArray(), Operation.Delete));
                    Redo_Buffer.Clear();
                }

                foreach (GameObject GO in SelectedGOs)
                    GO.ShouldBeRemoved = true;
            }

            //Some Scene window functionalities
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right) && ImGui.IsWindowFocused() && ImGui.IsWindowHovered())
                ImGui.OpenPopup("Functionalities");

            //Copy GOs
            if (ImGui.GetIO().KeyCtrl && ImGui.IsWindowFocused())
            {
                if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.C))) //Copy
                {
                    if (SelectedGOs.Count != 0)
                        GOs_Clipboard = SelectedGOs.ToArray();

                    IsCopy = true;
                }
                else if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.X))) //Cut
                {
                    if (SelectedGOs.Count != 0)
                    {
                        GOs_Clipboard = SelectedGOs.ToArray();
                        AddToACircularBuffer(Undo_Buffer, new KeyValuePair<object, Operation>(GOs_Clipboard, Operation.Delete));
                        Redo_Buffer.Clear();

                        foreach (GameObject GO in SelectedGOs)
                            GO.ShouldBeRemoved = true;
                    }
                    IsCopy = false;
                }
                else if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.D)) //Duplicate //Not functioning well...
                {
                    GameObject[] Instances = SelectedGOs.ToArray();
                    for (int i = 0; i < SelectedGOs.Count; i++)
                    {
                        Instances[i] = GameObject.Instantiate(Instances[i]);
                        Instances[i].Start();
                    }

                    if (SelectedGOs.Count != 0)
                    {
                        AddToACircularBuffer(Undo_Buffer, new KeyValuePair<object, Operation>(Instances, Operation.Create));
                        Redo_Buffer.Clear();
                    }
                }
                else if(ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.Z), true) && Undo_Buffer.Count != 0) //Handling Undo and Redo actions
                {
                    KeyValuePair<object, Operation> KVP = RemoveFromACircularBuffer(Undo_Buffer);
                    AddToACircularBuffer(Redo_Buffer, KVP);

                    object GOs = KVP.Key;
                    switch (KVP.Value)
                    {
                        case Operation.Create:
                            if (GOs.GetType().IsArray)
                            {
                                GameObject[] gameObjects = GOs as GameObject[];

                                for (int i = 0; i < gameObjects.Length; i++)
                                    gameObjects[i].ShouldBeRemoved = true;
                            }
                            else
                                (GOs as GameObject).ShouldBeRemoved = true;

                            break;
                        case Operation.Delete:
                            if (GOs.GetType().IsArray)
                            {
                                GameObject[] gameObjects = GOs as GameObject[];

                                for (int i = 0; i < gameObjects.Length; i++)
                                {
                                    if(gameObjects[i].PrevParent != null)
                                        gameObjects[i].PrevParent.AddChild(gameObjects[i]);
                                    SceneManager.ActiveScene.AddGameObject_Recursive(gameObjects[i]);
                                }
                            }
                            else
                            {
                                GameObject GO = GOs as GameObject;
                                if (GO.PrevParent != null)
                                    GO.PrevParent.AddChild(GO);
                                SceneManager.ActiveScene.AddGameObject_Recursive(GO);
                            }

                            break;
                    }
                }
                else if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.Y), true) && Redo_Buffer.Count != 0) //Handling Undo and Redo actions
                {
                    KeyValuePair<object, Operation> KVP = RemoveFromACircularBuffer(Redo_Buffer);
                    AddToACircularBuffer(Undo_Buffer, KVP);

                    object GOs = KVP.Key;
                    switch (KVP.Value)
                    {
                        case Operation.Create:
                            if (GOs.GetType().IsArray)
                            {
                                GameObject[] gameObjects = GOs as GameObject[];

                                for (int i = 0; i < gameObjects.Length; i++)
                                {
                                    if (gameObjects[i].PrevParent != null)
                                        gameObjects[i].PrevParent.AddChild(gameObjects[i]);
                                    SceneManager.ActiveScene.AddGameObject_Recursive(gameObjects[i]);
                                }
                            }
                            else
                            {
                                GameObject GO = GOs as GameObject;
                                if (GO.PrevParent != null)
                                    GO.PrevParent.AddChild(GO);
                                SceneManager.ActiveScene.AddGameObject_Recursive(GO);
                            }

                            break;
                        case Operation.Delete:
                            if (GOs.GetType().IsArray)
                            {
                                GameObject[] gameObjects = GOs as GameObject[];

                                for (int i = 0; i < gameObjects.Length; i++)
                                    gameObjects[i].ShouldBeRemoved = true;
                            }
                            else
                                (GOs as GameObject).ShouldBeRemoved = true;

                            break;
                    }
                }
                else if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.V))) //Paste
                {
                    if (GOs_Clipboard != null)
                    {
                        AddToACircularBuffer(Undo_Buffer, new KeyValuePair<object, Operation>(GOs_Clipboard, Operation.Create));
                        Redo_Buffer.Clear();

                        GameObject Parent = null;
                        if (SelectedGOs.Count == 1)
                            Parent = SelectedGOs.ToArray()[0];

                        if (IsCopy)
                        {
                            foreach (GameObject GO in GOs_Clipboard)
                            {
                                GameObject Instance = GameObject.Instantiate(GO);
                                
                                if (Parent != null)
                                {
                                    if (Instance.Parent != null)
                                        Instance.Parent.RemoveChild(Instance);
                                    Parent.AddChild(Instance);
                                }
                                else
                                {
                                    if (Instance.Parent != null)
                                        Instance.Parent.RemoveChild(Instance);
                                }

                                Instance.Start();
                            }
                        }
                        else
                        {
                            foreach (GameObject GO in GOs_Clipboard)
                            {
                                GameObject Instance = GameObject.Instantiate(GO);
                                if (Parent != null)
                                {
                                    if (Instance.Parent != null)
                                        Instance.Parent.RemoveChild(Instance);
                                    Parent.AddChild(Instance);
                                }
                                else
                                {
                                    if (Instance.Parent != null)
                                        Instance.Parent.RemoveChild(Instance);
                                }

                                Instance.Name = GO.Name;
                                Instance.Start();
                            }

                            GOs_Clipboard = null;
                        }
                    }
                }
                else if(ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.A))) //Select All
                {
                    foreach (GameObject GO in SceneManager.ActiveScene.GameObjects)
                        if(!GO.IsEditor)
                            SelectedGOs.Add(GO);
                }
            }
            if (ImGui.BeginPopup("Functionalities"))
            {
                if (ImGui.Selectable("New GameObject"))
                {
                    GameObject GO = new GameObject();
                    GO.Name = "Unique Name";
                    GO.AddComponent(new Transform());
                    if (WhoIsSelected != null)
                        WhoIsSelected.AddChild(GO);

                    GO.Start();

                    SceneManager.ActiveScene.AddGameObject(GO);

                    AddToACircularBuffer(Undo_Buffer, new KeyValuePair<object, Operation>(GO, Operation.Create));
                    Redo_Buffer.Clear();

                    //Should sort game objects?
                }

                ImGui.EndPopup();
            }

            if (ImGui.BeginPopup("Renaming"))
            {
                ImGui.Text("Edit name:");
                ImGui.InputText("##edit", ref NameBuffer, 50, ImGuiInputTextFlags.AutoSelectAll);
                if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.Enter)))
                {
                    if (NameBuffer.Replace(" ", "").Length > 0)
                        WhoIsSelected.Name = NameBuffer;
                    ImGui.CloseCurrentPopup();

                    AddToACircularBuffer(Undo_Buffer, new KeyValuePair<object, Operation>(NameBuffer, Operation.Rename));
                    Redo_Buffer.Clear();
                }

                ImGui.EndPopup();
            }

            //Renaming a GameObject
            if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.F2) && ImGui.IsWindowFocused())
            {
                if (WhoIsSelected != null)
                {
                    ImGui.OpenPopup("Renaming");
                    NameBuffer = WhoIsSelected.Name;
                }
            }

            ImGui.End();

            //ImGui.ShowDemoWindow();
        }

        private void AddToACircularBuffer(LinkedList<KeyValuePair<object, Operation>> Buffer, KeyValuePair<object, Operation> KVP)
        {
            if (Buffer.Count >= BufferLimit) // OverFlow
            {
                GameObject Deleted = Buffer.Last.Value.Key as GameObject;
                if (Deleted != null)
                    Deleted.Destroy();
                Buffer.RemoveLast();
            }

            Buffer.AddFirst(KVP);
        }

        private KeyValuePair<object, Operation> RemoveFromACircularBuffer(LinkedList<KeyValuePair<object, Operation>> Buffer)
        {
            if (Buffer.Count > 0) // Empty
            {
                var KVP = Buffer.First.Value;
                Buffer.RemoveFirst();
                return KVP;
            }

            return default(KeyValuePair<object, Operation>);
        }

        private void TreeRecursive(GameObject GO, bool Root)
        {
            if (GO.ShouldBeDeleted || GO.IsEditor)
                return;

            int ChildrenCount = GO.Children.Count;

            if(ChildrenCount == 0)
            {
                if (!Root)
                    ImGui.Indent();

                if (!GO.IsActive())
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1));
                    ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Vector4(0.8f, 0.8f, 0.8f, 1));
                }

                ImGui.Selectable(GO.Name, SelectedGOs.Contains(GO));

                if (!GO.IsActive())
                {
                    ImGui.PopStyleColor();
                    ImGui.PopStyleColor();
                }

                if (!Root)
                    ImGui.Unindent();

                if (ImGui.IsItemClicked(ImGuiMouseButton.Left) || ImGui.IsItemClicked(ImGuiMouseButton.Right))
                {
                    if (ImGui.GetIO().KeyCtrl)
                    {
                        if (!SelectedGOs.Add(GO))
                            SelectedGOs.Remove(GO);
                        else
                            WhoIsSelected = GO;
                    }
                    else
                    {
                        SelectedGOs.Clear();
                        if (!SelectedGOs.Add(GO))
                            SelectedGOs.Remove(GO);
                        else
                            WhoIsSelected = GO;
                    }

                    if (SelectedGOs.Count > 1)
                        WhoIsSelected = null;
                }

                return;
            }

            if (Root)
                ImGui.Unindent();

            if (!GO.IsActive())
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1));
                ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Vector4(0.8f, 0.8f, 0.8f, 1));
            }

            bool Open = ImGui.TreeNodeEx(GO.Name, SelectedGOs.Contains(GO) ? ImGuiTreeNodeFlags.Selected : ImGuiTreeNodeFlags.None);

            if (!GO.IsActive())
            {
                ImGui.PopStyleColor();
                ImGui.PopStyleColor();
            }

            if (ImGui.IsItemClicked(ImGuiMouseButton.Left) || ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                if (ImGui.GetIO().KeyCtrl)
                {
                    if (!SelectedGOs.Add(GO))
                        SelectedGOs.Remove(GO);
                    else
                        WhoIsSelected = GO;
                }
                else
                {
                    SelectedGOs.Clear();
                    if (!SelectedGOs.Add(GO))
                        SelectedGOs.Remove(GO);
                    else
                        WhoIsSelected = GO;
                }

                if (SelectedGOs.Count > 1)
                    WhoIsSelected = null;
            }

            for (int i = 0; i < ChildrenCount; i++)
                if(Open)
                    TreeRecursive(GO.Children[i], false);

            if (Open)
                ImGui.TreePop();

            if(Root)
                ImGui.Indent();
        }
    }
}
