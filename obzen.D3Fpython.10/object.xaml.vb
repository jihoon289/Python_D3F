Public Class objectPRD
    Public Sub New()
        InitializeComponent()
    End Sub

    Private _ID As Integer
    Private _Name As String
    Private _Alias As String
    Private _Type As String
    Private _ExReVisible As Boolean
    Private _isSub As String
    Private _MasterName As String
    Private _MasterID As Integer
    Private _Kind As String
    '[[ Type ]]
    'DataSet_S
    'DataSet_M
    'Field_Str
    'Field_Int
    'Model
    'Etc
    'String

    '[[ Kind ]]
    'PY : Python
    'DS : DataSet


    Public Property SetID As Integer
        Get
            Return _ID
        End Get
        Set(value As Integer)
            _ID = value
        End Set
    End Property

    Public Property SetMasterID As Integer
        Get
            Return _MasterID
        End Get
        Set(value As Integer)
            _MasterID = value
        End Set
    End Property

    Public Property KIND_P As String
        Get
            Return _Kind
        End Get
        Set(value As String)
            _Kind = value
        End Set
    End Property



    Public Property ExReVisible As Boolean
        Get
            Return _ExReVisible
        End Get
        Set(value As Boolean)
            _ExReVisible = value
            If value Then
                FirstCol.Width = New GridLength(15)
                'ExRe.Visibility = Visibility.Visible
            Else
                FirstCol.Width = New GridLength(0)
                'ExRe.Visibility = Visibility.Collapsed
            End If
        End Set
    End Property

    Public Property isSub As String
        Get
            Return _isSub
        End Get
        Set(value As String)
            _isSub = value
            If value.Equals("1") And _ExReVisible Then
                FirstCol.Width = New GridLength(15)
                'ExRe.Visibility = Visibility.Visible
            Else
                FirstCol.Width = New GridLength(0)
                'ExRe.Visibility = Visibility.Collapsed
            End If
        End Set
    End Property

    Public Property Name_P As String
        Get
            Return _Name
        End Get
        Set(value As String)
            _Name = value
            'ObjLB.Content = _Name
        End Set
    End Property

    Public Property MasterName_P As String
        Get
            Return _MasterName
        End Get
        Set(value As String)
            _MasterName = value
        End Set
    End Property

    Public Property Alias_P As String
        Get
            Return _Alias
        End Get
        Set(value As String)
            _Alias = value
            ObjLB.Text = _Alias
        End Set
    End Property

    Public Property ViewName As Boolean
        Get
            Return ObjLB.Text
        End Get
        Set(value As Boolean)
            If value Then
                ObjLB.Text = _Name
            Else
                ObjLB.Text = _Alias
            End If
        End Set
    End Property

    Public Property Type_P As String
        Get
            Return _Type
        End Get
        Set(value As String)
            _Type = value

            If (value.Equals("DataSet_S")) Then
                ObjImg.Source = New BitmapImage(New Uri("/obzen.D3Fpython.10;component/Image/DataSet.png", UriKind.Relative))
            ElseIf (value.Equals("DataSet_M")) Then
                ObjImg.Source = New BitmapImage(New Uri("/obzen.D3Fpython.10;component/Image/SegDataSet.png", UriKind.Relative))
            ElseIf (value.Equals("Model") Or value.Equals("Model_S")) Then
                ObjImg.Source = New BitmapImage(New Uri("/obzen.D3Fpython.10;component/Image/Model.png", UriKind.Relative))
            ElseIf (value.Equals("Model_M")) Then
                ObjImg.Source = New BitmapImage(New Uri("/obzen.D3Fpython.10;component/Image/Model_M.png", UriKind.Relative))
            ElseIf (value.Equals("Etc") Or value.Equals("Etc_S")) Then
                ObjImg.Source = New BitmapImage(New Uri("/obzen.D3Fpython.10;component/Image/ETC.png", UriKind.Relative))
            ElseIf (value.Equals("Etc_M")) Then
                ObjImg.Source = New BitmapImage(New Uri("/obzen.D3Fpython.10;component/Image/ETC_M.png", UriKind.Relative))
            ElseIf (value.Equals("String")) Then
                ObjImg.Source = New BitmapImage(New Uri("/obzen.D3Fpython.10;component/Image/String.png", UriKind.Relative))
            ElseIf (value.Equals("Field_Str")) Then
                ObjImg.Source = New BitmapImage(New Uri("/obzen.D3Fpython.10;component/Image/Type_String.png", UriKind.Relative))
                Grd.Margin = New Thickness(13, 2, 5, 2)
                Grd.Background = New SolidColorBrush(Color.FromRgb(232, 239, 226))
                Grd.Height = 26
            ElseIf (value.Equals("Field_Int")) Then
                ObjImg.Source = New BitmapImage(New Uri("/obzen.D3Fpython.10;component/Image/Type_Number.png", UriKind.Relative))
                Grd.Margin = New Thickness(13, 2, 5, 2)
                Grd.Background = New SolidColorBrush(Color.FromRgb(232, 239, 226))
                Grd.Height = 26
            End If
        End Set
    End Property

    Private Expand As Boolean = True
    Public Event ExReClick(sender As objectPRD, e As Boolean)
    Private Sub ExRe_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles ExRe.MouseDown
        If Expand Then
            ExRe.Source = New BitmapImage(New Uri("/obzen.D3Fpython.10;component/Image/Expand.png", UriKind.Relative))
            Expand = False
        Else
            ExRe.Source = New BitmapImage(New Uri("/obzen.D3Fpython.10;component/Image/16_Reduce.png", UriKind.Relative))
            Expand = True
        End If
        RaiseEvent ExReClick(Me, Not Expand)
    End Sub
End Class
