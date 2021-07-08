using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using System.Linq;
using System;
using System.Reflection;

namespace FN_Engine.FN_Editor
{
    enum Operation { Delete, Create, Rename, GO_DragAndDrop, AddComponent, RemoveComponent, ChangeValue };

    internal class GameObjects_Tab : GameObjectComponent
    {
        public static GameObject WhoIsSelected = null;
        public static Vector2[] MyRegion;
        public static LinkedList<KeyValuePair<object, Operation>> Undo_Buffer;
        public static LinkedList<KeyValuePair<object, Operation>> Redo_Buffer;

        public const int BufferLimit = 200; //200 Items

        private string NameBuffer = "";
        private HashSet<GameObject> SelectedGOs;
        private GameObject[] GOs_Clipboard = null;
        private bool IsCopy = true;
        private GameObject DraggedGO;

        public override void Start()
        {
            DraggedGO = null;
            WhoIsSelected = null;
            SelectedGOs = new HashSet<GameObject>();
            Undo_Buffer = new LinkedList<KeyValuePair<object, Operation>>();
            Redo_Buffer = new LinkedList<KeyValuePair<object, Operation>>();
            LinkedList<int> T = new LinkedList<int>();
            MyRegion = new Vector2[2];
        }

        public override void DrawUI()
        {
            //Debug
            ImGui.Text("Undo Buffer Count: ");
            ImGui.SameLine();
            ImGui.Text(Undo_Buffer.Count.ToString());

            ImGui.Text("Redo Buffer Count: ");
            ImGui.SameLine();
            ImGui.Text(Redo_Buffer.Count.ToString());

            ImGui.Text("Mouse Pos: ");
            ImGui.SameLine();
            ImGui.Text(Input.GetMousePosition().ToString());

            ImGui.Text("Game Running Slowly: ");
            //ImGui.SameLine();

            ImGui.Text(Setup.Camera.Position.ToString());
            /////

            //Scene Tab
            ImGui.Begin(SceneManager.ActiveScene.Name);

            ///
            if (MyRegion[1].Y != 0)
            {
                float DeltaSize = MyRegion[1].Y - ImGui.GetWindowSize().Y;

                if (DeltaSize != 0)
                {
                    ImGui.SetWindowSize("Content Manager", ContentWindow.MyRegion[1] + new Vector2(0, DeltaSize));
                    ImGui.SetWindowPos("Content Manager", ContentWindow.MyRegion[0] - new Vector2(0, DeltaSize));
                }
            }

            MyRegion[0] = ImGui.GetWindowPos();
            MyRegion[1] = ImGui.GetWindowSize();
            ///

            ImGui.Indent();

            for (int i = SceneManager.ActiveScene.GameObjects.Count - 1; i >= 0; i--)
            {
                GameObject GO = SceneManager.ActiveScene.GameObjects[i];
                if (!GO.IsEditor && GO.Parent == null && !GO.ShouldBeDeleted)
                    TreeRecursive(GO, true);
            }

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
            if (ImGui.GetIO().KeyCtrl)
            {
                if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.C))) //Copy
                {
                    if (SelectedGOs.Count != 0)
                    {
                        GOs_Clipboard = SelectedGOs.ToArray();

                        for (int i = 0; i < GOs_Clipboard.Length; i++)
                        {
                            string OrignalName = GOs_Clipboard[i].Name;

                            string[] OrigNames = new string[GOs_Clipboard.Length];
                            GameObject[] AllChildren = GOs_Clipboard[i].GetALLChildren();

                            if (AllChildren != null)
                                for (int j = 0; j < AllChildren.Length; j++)
                                    OrigNames[j] = AllChildren[j].Name;

                            GOs_Clipboard[i] = GameObject.Instantiate(GOs_Clipboard[i]);
                            GOs_Clipboard[i].Name = OrignalName;

                            AllChildren = GOs_Clipboard[i].GetALLChildren();
                            if (AllChildren != null)
                                for (int j = 0; j < AllChildren.Length; j++)
                                    AllChildren[j].Name = OrigNames[j];

                            SceneManager.ActiveScene.RemoveGameObject(GOs_Clipboard[i], false);
                        }
                    }

                    IsCopy = true;
                }
                else if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.X))) //Cut
                {
                    if (SelectedGOs.Count != 0)
                    {
                        GOs_Clipboard = SelectedGOs.ToArray();
                        Redo_Buffer.Clear();

                        if (SelectedGOs.Count != 0)
                        {
                            AddToACircularBuffer(Undo_Buffer, new KeyValuePair<object, Operation>(GOs_Clipboard, Operation.Delete));
                            Redo_Buffer.Clear();
                        }

                        foreach (GameObject GO in SelectedGOs)
                            GO.ShouldBeRemoved = true;
                    }
                    IsCopy = false;
                }
                else if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.D)) //Duplicate //Not functioning well...
                {
                    GameObject[] Instances = SelectedGOs.ToArray();
                    for (int i = 0; i < SelectedGOs.Count; i++)
                        Instances[i] = GameObject.Instantiate(Instances[i]);

                    if (SelectedGOs.Count != 0)
                    {
                        AddToACircularBuffer(Undo_Buffer, new KeyValuePair<object, Operation>(Instances, Operation.Create));
                        Redo_Buffer.Clear();
                    }
                }
                else if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.Z), true) && Undo_Buffer.Count != 0) //Handling Undo and Redo actions
                {
                    KeyValuePair<object, Operation> KVP = RemoveFromACircularBuffer(Undo_Buffer);

                    object GOs = KVP.Key;
                    switch (KVP.Value)
                    {
                        case Operation.Create:
                            AddToACircularBuffer(Redo_Buffer, KVP);

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
                            AddToACircularBuffer(Redo_Buffer, KVP);

                            if (GOs.GetType().IsArray)
                            {
                                GameObject[] gameObjects = GOs as GameObject[];

                                for (int i = 0; i < gameObjects.Length; i++)
                                {
                                    if (gameObjects[i].PrevParent != null)
                                        gameObjects[i].PrevParent.AddChild(gameObjects[i]);

                                    //gameObjects[i].Name = Utility.UniqueGameObjectName(gameObjects[i].Name);
                                    SceneManager.ActiveScene.AddGameObject_Recursive(gameObjects[i], true, true);
                                }
                            }
                            else
                            {
                                GameObject GO = GOs as GameObject;
                                if (GO.PrevParent != null)
                                    GO.PrevParent.AddChild(GO);

                                //GO.Name = Utility.UniqueGameObjectName(GO.Name);
                                SceneManager.ActiveScene.AddGameObject_Recursive(GO, true, true);
                            }

                            break;
                        case Operation.GO_DragAndDrop:
                            AddToACircularBuffer(Redo_Buffer, KVP);

                            KeyValuePair<object, GameObject> KVP_GO = (KeyValuePair<object, GameObject>)KVP.Key;
                            KeyValuePair<GameObject, string> SourceAndPrevParent = (KeyValuePair<GameObject, string>)KVP_GO.Key;
                            GameObject PrevParent = SceneManager.ActiveScene.FindGameObjectWithName(SourceAndPrevParent.Value);
                            if (KVP_GO.Value == null) //Destination is null
                                PrevParent.AddChild(SourceAndPrevParent.Key);
                            else
                            {
                                if (PrevParent != null)
                                    PrevParent.AddChild(SourceAndPrevParent.Key);

                                KVP_GO.Value.RemoveChild(SourceAndPrevParent.Key);
                            }

                            break;
                        case Operation.Rename:
                            KeyValuePair<GameObject, string> NewKVP = (KeyValuePair<GameObject, string>)KVP.Key;
                            AddToACircularBuffer(Redo_Buffer, new KeyValuePair<object, Operation>(new KeyValuePair<GameObject, string>(NewKVP.Key, NewKVP.Key.Name), Operation.Rename));

                            NewKVP.Key.Name = NewKVP.Value;

                            break;
                        case Operation.AddComponent:
                            KeyValuePair<GameObject, GameObjectComponent> Info = (KeyValuePair<GameObject, GameObjectComponent>)GOs;
                            AddToACircularBuffer(Redo_Buffer, KVP);
                            Info.Key.RemoveComponent(Info.Value, false);
                            WhoIsSelected = Info.Key;

                            break;
                        case Operation.RemoveComponent:
                            KeyValuePair<GameObject, GameObjectComponent> InfoRemove = (KeyValuePair<GameObject, GameObjectComponent>)GOs;
                            AddToACircularBuffer(Redo_Buffer, KVP);
                            InfoRemove.Key.AddComponent(InfoRemove.Value);
                            WhoIsSelected = InfoRemove.Key;

                            break;
                        case Operation.ChangeValue:
                            KeyValuePair<object, object> ValueChanged = (KeyValuePair<object, object>)GOs;
                            KeyValuePair<object, object> ObjectAndField = (KeyValuePair<object, object>)ValueChanged.Key;

                            if (ObjectAndField.Value is FieldInfo)
                            {
                                AddToACircularBuffer(Redo_Buffer, new KeyValuePair<object, Operation>(new KeyValuePair<object, object>(ObjectAndField, ((FieldInfo)ObjectAndField.Value).GetValue(ObjectAndField.Key)), Operation.ChangeValue));
                                ((FieldInfo)ObjectAndField.Value).SetValue(ObjectAndField.Key, ValueChanged.Value);
                            }
                            else
                            {
                                AddToACircularBuffer(Redo_Buffer, new KeyValuePair<object, Operation>(new KeyValuePair<object, object>(ObjectAndField, ((PropertyInfo)ObjectAndField.Value).GetValue(ObjectAndField.Key)), Operation.ChangeValue));
                                ((PropertyInfo)ObjectAndField.Value).SetValue(ObjectAndField.Key, ValueChanged.Value);
                            }

                            break;
                    }
                }
                else if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.Y), true) && Redo_Buffer.Count != 0) //Handling Undo and Redo actions
                {
                    KeyValuePair<object, Operation> KVP = RemoveFromACircularBuffer(Redo_Buffer);

                    object GOs = KVP.Key;
                    switch (KVP.Value)
                    {
                        case Operation.Create:
                            AddToACircularBuffer(Undo_Buffer, KVP);

                            if (GOs.GetType().IsArray)
                            {
                                GameObject[] gameObjects = GOs as GameObject[];

                                for (int i = 0; i < gameObjects.Length; i++)
                                {
                                    if (gameObjects[i].PrevParent != null)
                                        gameObjects[i].PrevParent.AddChild(gameObjects[i]);

                                    SceneManager.ActiveScene.AddGameObject_Recursive(gameObjects[i], true, true);
                                }
                            }
                            else
                            {
                                GameObject GO = GOs as GameObject;
                                if (GO.PrevParent != null)
                                    GO.PrevParent.AddChild(GO);

                                SceneManager.ActiveScene.AddGameObject_Recursive(GO, true, true);
                            }

                            break;
                        case Operation.Delete:
                            AddToACircularBuffer(Undo_Buffer, KVP);

                            if (GOs.GetType().IsArray)
                            {
                                GameObject[] gameObjects = GOs as GameObject[];

                                for (int i = 0; i < gameObjects.Length; i++)
                                    gameObjects[i].ShouldBeRemoved = true;
                            }
                            else
                                (GOs as GameObject).ShouldBeRemoved = true;

                            break;
                        case Operation.GO_DragAndDrop:
                            AddToACircularBuffer(Undo_Buffer, KVP);

                            KeyValuePair<object, GameObject> KVP_GO = (KeyValuePair<object, GameObject>)KVP.Key;
                            KeyValuePair<GameObject, string> SourceAndPrevParent = (KeyValuePair<GameObject, string>)KVP_GO.Key;
                            GameObject PrevParent = SceneManager.ActiveScene.FindGameObjectWithName(SourceAndPrevParent.Value);
                            if (KVP_GO.Value == null && SourceAndPrevParent.Key.Parent != null) //Destination is null
                                SourceAndPrevParent.Key.Parent.RemoveChild(SourceAndPrevParent.Key);
                            else if (PrevParent != null)
                                PrevParent.AddChild(SourceAndPrevParent.Key);

                            break;
                        case Operation.Rename:
                            KeyValuePair<GameObject, string> NewKVP = (KeyValuePair<GameObject, string>)KVP.Key;
                            AddToACircularBuffer(Undo_Buffer, new KeyValuePair<object, Operation>(new KeyValuePair<GameObject, string>(NewKVP.Key, NewKVP.Key.Name), Operation.Rename));

                            NewKVP.Key.Name = NewKVP.Value;

                            break;
                        case Operation.AddComponent:
                            KeyValuePair<GameObject, GameObjectComponent> Info = (KeyValuePair<GameObject, GameObjectComponent>)GOs;
                            AddToACircularBuffer(Undo_Buffer, KVP);
                            Info.Key.AddComponent(Info.Value);
                            WhoIsSelected = Info.Key;

                            break;
                        case Operation.RemoveComponent:
                            KeyValuePair<GameObject, GameObjectComponent> InfoRemove = (KeyValuePair<GameObject, GameObjectComponent>)GOs;
                            AddToACircularBuffer(Undo_Buffer, KVP);
                            InfoRemove.Key.RemoveComponent(InfoRemove.Value, false);
                            WhoIsSelected = InfoRemove.Key;

                            break;
                        case Operation.ChangeValue:
                            KeyValuePair<object, object> ValueChanged = (KeyValuePair<object, object>)GOs;
                            KeyValuePair<object, object> ObjectAndField = (KeyValuePair<object, object>)ValueChanged.Key;

                            if (ObjectAndField.Value is FieldInfo)
                            {
                                AddToACircularBuffer(Undo_Buffer, new KeyValuePair<object, Operation>(new KeyValuePair<object, object>(ObjectAndField, ((FieldInfo)ObjectAndField.Value).GetValue(ObjectAndField.Key)), Operation.ChangeValue));
                                ((FieldInfo)ObjectAndField.Value).SetValue(ObjectAndField.Key, ValueChanged.Value);
                            }
                            else
                            {
                                AddToACircularBuffer(Undo_Buffer, new KeyValuePair<object, Operation>(new KeyValuePair<object, object>(ObjectAndField, ((PropertyInfo)ObjectAndField.Value).GetValue(ObjectAndField.Key)), Operation.ChangeValue));
                                ((PropertyInfo)ObjectAndField.Value).SetValue(ObjectAndField.Key, ValueChanged.Value);
                            }

                            break;
                    }
                }
                else if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.V))) //Paste
                {
                    if (GOs_Clipboard != null)
                    {
                        GameObject Parent = null;
                        if (SelectedGOs.Count == 1)
                            Parent = SelectedGOs.ToArray()[0];

                        GameObject[] Instances = new GameObject[GOs_Clipboard.Length];
                        for (int i = 0; i < GOs_Clipboard.Length; i++)
                        {
                            GameObject Instance = GameObject.Instantiate(GOs_Clipboard[i]);
                            Instances[i] = Instance;
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
                        }

                        if (!IsCopy)
                            GOs_Clipboard = null;

                        AddToACircularBuffer(Undo_Buffer, new KeyValuePair<object, Operation>(Instances, Operation.Create));
                        Redo_Buffer.Clear();
                    }
                }
                else if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.A))) //Select All
                {
                    foreach (GameObject GO in SceneManager.ActiveScene.GameObjects)
                        if (!GO.IsEditor)
                            SelectedGOs.Add(GO);
                }
            }
            if (ImGui.BeginPopup("Functionalities"))
            {
                if (ImGui.Selectable("New GameObject"))
                {
                    GameObject GO = new GameObject();
                    //GO.Name = Utility.UniqueGameObjectName("Unique Name");
                    GO.AddComponent(new Transform());
                    GO.Transform = GO.GetComponent<Transform>();
                    if (WhoIsSelected != null)
                        WhoIsSelected.AddChild(GO);

                    GO.Start();

                    SceneManager.ActiveScene.AddGameObject_Recursive(GO);

                    AddToACircularBuffer(Undo_Buffer, new KeyValuePair<object, Operation>(GO, Operation.Create));
                    Redo_Buffer.Clear();

                    //Should sort game objects?
                }

                if (ImGui.Selectable("New Sprite"))
                {
                    GameObject GO = new GameObject();
                    //GO.Name = Utility.UniqueGameObjectName("Unique Name");
                    GO.AddComponent(new Transform());
                    GO.AddComponent(new SpriteRenderer());
                    GO.Transform = GO.GetComponent<Transform>();
                    if (WhoIsSelected != null)
                        WhoIsSelected.AddChild(GO);

                    GO.Start();

                    SceneManager.ActiveScene.AddGameObject_Recursive(GO);

                    AddToACircularBuffer(Undo_Buffer, new KeyValuePair<object, Operation>(GO, Operation.Create));
                    Redo_Buffer.Clear();

                    //Should sort game objects?
                }

                if (ImGui.Selectable("New Light"))
                {
                    GameObject GO = new GameObject();
                    //GO.Name = Utility.UniqueGameObjectName("Unique Name");
                    GO.AddComponent(new Transform());
                    GO.AddComponent(new Light());
                    GO.Transform = GO.GetComponent<Transform>();
                    if (WhoIsSelected != null)
                        WhoIsSelected.AddChild(GO);

                    GO.Start();

                    SceneManager.ActiveScene.AddGameObject_Recursive(GO);

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
                    if (NameBuffer.Replace(" ", "").Length > 0 && NameBuffer != WhoIsSelected.Name)
                    {
                        KeyValuePair<GameObject, string> BufferedObject = new KeyValuePair<GameObject, string>(WhoIsSelected, WhoIsSelected.Name);
                        AddToACircularBuffer(Undo_Buffer, new KeyValuePair<object, Operation>(BufferedObject, Operation.Rename));
                        Redo_Buffer.Clear();

                        NameBuffer = Utility.UniqueGameObjectName(NameBuffer);
                        WhoIsSelected.Name = NameBuffer;
                    }
                    ImGui.CloseCurrentPopup();
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

            if (ImGui.IsWindowHovered(ImGuiHoveredFlags.RootWindow) && !ImGui.IsAnyItemHovered())
            {
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && ImGui.IsWindowFocused())
                {
                    WhoIsSelected = null;
                    SelectedGOs.Clear();
                }

                if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                {
                    if (DraggedGO != null && DraggedGO.Parent != null)
                    {
                        KeyValuePair<object, GameObject> KVP_GO = new KeyValuePair<object, GameObject>(new KeyValuePair<GameObject, string>(DraggedGO, (DraggedGO.Parent == null) ? null : DraggedGO.Parent.Name), null);

                        DraggedGO.Parent.RemoveChild(DraggedGO);

                        AddToACircularBuffer(Undo_Buffer, new KeyValuePair<object, Operation>(KVP_GO, Operation.GO_DragAndDrop));
                        Redo_Buffer.Clear();
                    }

                    DraggedGO = null;
                }
            }

            ImGui.End();
        }

        public static void AddToACircularBuffer(LinkedList<KeyValuePair<object, Operation>> Buffer, KeyValuePair<object, Operation> KVP)
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

        public static KeyValuePair<object, Operation> RemoveFromACircularBuffer(LinkedList<KeyValuePair<object, Operation>> Buffer)
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

            if (ChildrenCount == 0)
            {
                if (!Root)
                    ImGui.Indent();

                if (!GO.IsActive())
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1));
                    ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Vector4(0.8f, 0.8f, 0.8f, 1));
                }

                ImGui.Selectable(GO.Name, SelectedGOs.Contains(GO));

                //Accept Drag and Drop
                if (ImGui.BeginDragDropSource(ImGuiDragDropFlags.SourceNoDisableHover))
                {
                    ImGui.SetDragDropPayload("GameObject", IntPtr.Zero, 0);
                    DraggedGO = GO;

                    ImGui.Text(GO.Name);

                    ImGui.EndDragDropSource();
                }

                // The GameObject is a drag source and drop target at the same time
                if (ImGui.BeginDragDropTarget())
                {
                    if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                    {
                        if (DraggedGO != null && GO.Parent != DraggedGO)
                        {
                            if (DraggedGO.Parent != null)
                                DraggedGO.Parent.RemoveChild(DraggedGO);
                            GO.AddChild(DraggedGO);

                            // Undo and Redo Buffering
                            KeyValuePair<object, GameObject> KVP_GO = new KeyValuePair<object, GameObject>(new KeyValuePair<GameObject, string>(DraggedGO, (DraggedGO.Parent == null) ? null : DraggedGO.Parent.Name), GO);
                            AddToACircularBuffer(Undo_Buffer, new KeyValuePair<object, Operation>(KVP_GO, Operation.GO_DragAndDrop));
                            Redo_Buffer.Clear();
                        }

                        DraggedGO = null;
                    }

                    ImGui.EndDragDropTarget();
                }

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

            //Accept Drag and Drop
            if (ImGui.BeginDragDropSource(ImGuiDragDropFlags.SourceNoDisableHover))
            {
                ImGui.SetDragDropPayload("GameObject", IntPtr.Zero, 0);
                DraggedGO = GO;

                ImGui.Text(GO.Name);

                ImGui.EndDragDropSource();
            }

            // The GameObject is a drag source and drop targer at the same time
            if (ImGui.BeginDragDropTarget())
            {
                if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                {
                    if (DraggedGO != null && GO.Parent != DraggedGO)
                    {
                        if (DraggedGO.Parent != null)
                            DraggedGO.Parent.RemoveChild(DraggedGO);
                        GO.AddChild(DraggedGO);

                        // Undo and Redo Buffering
                        KeyValuePair<object, GameObject> KVP_GO = new KeyValuePair<object, GameObject>(new KeyValuePair<GameObject, string>(DraggedGO, (DraggedGO.Parent == null) ? null : DraggedGO.Parent.Name), GO);
                        AddToACircularBuffer(Undo_Buffer, new KeyValuePair<object, Operation>(KVP_GO, Operation.GO_DragAndDrop));
                        Redo_Buffer.Clear();
                    }

                    DraggedGO = null;
                }

                ImGui.EndDragDropTarget();
            }

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
            {
                if (ChildrenCount != GO.Children.Count)
                    break;

                if (Open)
                    TreeRecursive(GO.Children[i], false);
            }

            if (Open)
                ImGui.TreePop();

            if (Root)
                ImGui.Indent();
        }
    }
}