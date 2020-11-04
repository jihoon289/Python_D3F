Public Class SegObject
    Private _SegName As String
    Private _SegID As String

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Property SegName As String
        Get
            Return _SegName
        End Get
        Set(value As String)
            _SegName = value
            SegButton.Content = value
        End Set
    End Property

    Public Property SegID As String
        Get
            Return _SegID
        End Get
        Set(value As String)
            _SegID = value
            SegButton.Tag = _SegID
        End Set
    End Property

    Public Sub SelectedYN(SelectYN As Boolean)
        If SelectYN Then
            SegButton.Background = Brushes.LightSkyBlue
        Else
            SegButton.Background = Brushes.White
        End If
    End Sub
End Class
