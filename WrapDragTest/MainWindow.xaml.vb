Class MainWindow
    Public Sub New()

        ' 디자이너에서 이 호출이 필요합니다.
        InitializeComponent()

        ' InitializeComponent() 호출 뒤에 초기화 코드를 추가하세요.

    End Sub

    Private Sub WrapPanel_DragEnter(sender As Object, e As DragEventArgs)
        e.Effects = DragDropEffects.Copy
    End Sub

    Private Sub StackPanel_DragOver(sender As Object, e As DragEventArgs)
        e.Effects = DragDropEffects.Copy
    End Sub


    Private Sub Label_MouseDown(sender As Object, e As MouseButtonEventArgs)
        DragDrop.DoDragDrop(sender, "bb", DragDropEffects.Move)
    End Sub

    Private Sub Label_MouseMove(sender As Object, e As MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            DragDrop.DoDragDrop(sender, "bb", DragDropEffects.Move)
        End If
    End Sub
End Class
