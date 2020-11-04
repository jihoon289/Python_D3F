Public Class FromDataSet_Simple
    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub ViewDroplabel()
        Lb_drop.Visibility = Visibility.Visible
    End Sub

    Public AllowDrag As Boolean = True
    Private NonSimple As Boolean = False
    Public IsSegment As Boolean = False

    Public Property ViewNonSimple As Boolean
        Get
            Return NonSimple
        End Get
        Set(value As Boolean)
            NonSimple = value
        End Set
    End Property

    Public Sub AddObject(Type As String, Name As String, _Alias As String, ID As Integer, KIND As String,
                         Optional isSub As String = "0", Optional MasterNM As String = "", Optional MasterID As Integer = 0, Optional ISUP_position As Boolean = False)
        Dim Obj As objectPRD = New objectPRD With {.Type_P = Type, .Name_P = Name, .Alias_P = _Alias, .ExReVisible = NonSimple,
            .isSub = isSub, .SetID = ID, .MasterName_P = MasterNM, .SetMasterID = MasterID, .KIND_P = KIND}

        AddHandler Obj.ExReClick, AddressOf ExRe_MouseDown
        AddHandler Obj.MouseDown, AddressOf ObjectDragStart

        '중복 막기
        Dim isdup As Boolean = False
        For Each ele As UIElement In ViewerPanel.Children
            Dim objdm As objectPRD = TryCast(ele, objectPRD)
            If objdm IsNot Nothing AndAlso (objdm.Name_P.Equals(Name) And objdm.Alias_P.Equals(_Alias)) Then
                If Type.Contains("Field") And objdm.Type_P.Contains("Field") And objdm.MasterName_P.Equals(MasterNM) Then
                    isdup = True
                ElseIf Not Type.Contains("Field") Then
                    isdup = True
                End If
            End If
        Next

        If isdup = False Then
            If ISUP_position Then
                ViewerPanel.Children.Insert(0, Obj)
            Else
                ViewerPanel.Children.Add(Obj)
            End If
        End If
    End Sub

    Public Sub RemoveObject(Name As String)
        For Each ele As UIElement In ViewerPanel.Children
            Dim objdm As objectPRD = TryCast(ele, objectPRD)
            If objdm IsNot Nothing AndAlso (objdm.Name_P.Equals(Name)) Then
                ViewerPanel.Children.Remove(objdm)
            End If
        Next
    End Sub
    Public Sub RemoveObject(ID As Integer)
        For Each ele As UIElement In ViewerPanel.Children
            Dim objdm As objectPRD = TryCast(ele, objectPRD)
            If objdm IsNot Nothing AndAlso (objdm.SetID.Equals(ID)) Then
                ViewerPanel.Children.Remove(objdm)
            End If
        Next
    End Sub

    Public Event ExReClick(sender As objectPRD, e As Boolean)
    Private IsExReUsing As Boolean = False
    Public Sub ExRe_MouseDown(sender As objectPRD, e As Boolean)
        IsExReUsing = True
        RaiseEvent ExReClick(sender, Not e)
    End Sub

    Public Sub ObjectDragStart(sender As objectPRD, e As MouseEventArgs)
        If AllowDrag Then
            If IsSegment = False And IsExReUsing = False Then
                Dim eData As String = ""
                If sender.Type_P.Contains("DataSet") Then
                    eData = sender.Name_P
                ElseIf sender.Type_P.Contains("Field") Then
                    eData = sender.MasterName_P & "['" & sender.Name_P & "']"
                Else
                    eData = sender.Name_P
                End If
                DragDrop.DoDragDrop(sender, eData, DragDropEffects.Move)
            ElseIf IsSegment And IsExReUsing = False Then
                DragDrop.DoDragDrop(sender, sender, DragDropEffects.Move)
            End If
            IsExReUsing = False
        End If
    End Sub

    'Public Event ObjectDrop(ColName As objectPRD, e As EventArgs)

    'Private Sub ViewerPanel_Drop(sender As Object, e As DragEventArgs) Handles ViewerPanel.Drop
    '    Dim DragObj As objectPRD = e.Data.GetData(GetType(objectPRD))
    '    If Not DragObj Is Nothing Then
    '        If DragObj.Type_P.Contains("Field") Then
    '            ViewerPanel.Children.Clear()
    '            Dim Obj As objectPRD = New objectPRD With {.Type_P = DragObj.Type_P, .Name_P = DragObj.Name_P, .Alias_P = DragObj.Alias_P,
    '            .ExReVisible = DragObj.ExReVisible, .isSub = DragObj.isSub, .SetID = DragObj.SetID, .MasterName_P = DragObj.MasterName_P, .SetMasterID = DragObj.SetMasterID}
    '            ViewerPanel.Children.Add(Obj)
    '            RaiseEvent ObjectDrop(DragObj, Nothing)
    '        End If
    '    End If
    'End Sub
End Class
