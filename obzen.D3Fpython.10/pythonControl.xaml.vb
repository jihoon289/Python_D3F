Imports System.IO

Public Class pythonControl

    Private InputListPara_backup As String
    Private SingleCnt As Integer = 0
    Private MultiCnt As Integer = 0
    Private ModelCnt As Integer = 0
    Private ETCCnt As Integer = 0
    Private StringCnt As Integer = 0
    Private IntegerCnt As Integer = 0
    'Private MasterSub As New Dictionary(Of String, List(Of String))   'Master Name의 Sub Name들 (table명 Column명)
    Private OnloadBool As Boolean = True

    Public Sub New()

        ' 디자이너에서 이 호출이 필요합니다.
        InitializeComponent()

        InputViewer.ViewNonSimple = False
        InputList.ViewNonSimple = True
        SegColList.ViewNonSimple = True
        SegCol.ViewDroplabel()

        RemoveHandler InputList.ExReClick, AddressOf ExRe_MouseDown
        AddHandler InputList.ExReClick, AddressOf ExRe_MouseDown

        RemoveHandler SegColList.ExReClick, AddressOf ExRe_MouseDown_Seg
        AddHandler SegColList.ExReClick, AddressOf ExRe_MouseDown_Seg

        'RemoveHandler SegCol.ObjectDrop, AddressOf ObjectDropFunction
        'AddHandler SegCol.ObjectDrop, AddressOf ObjectDropFunction

        Editor_header.Code = "#-*- coding:utf-8 -*-" & vbNewLine & "import pandas" & vbNewLine & "import sys" & vbNewLine & "import pickle"

        Segment.Visibility = Visibility.Collapsed
    End Sub

    Public Event SegDrop(SegColInfo As Object, EventName As String)
    Public SegColName As String = ""
    Public SegTBLName As String = ""
    Public SegTBLKind As String = ""

    Public Sub ScreenMode()
        AddInputFileBorder.Visibility = Visibility.Hidden
        rdo_ExWay.IsEnabled = False
        rdo_ExType.IsEnabled = False
        txt_NM.IsReadOnly = True
        txt_Desc.IsReadOnly = True

        InputViewer.AllowDrag = False
        SegColList.AllowDrag = False
        SegCol.AllowDrag = False
        InputList.AllowDrag = False

        Editor_header.isReadonly = True
        Editor.isReadonly = True

        img1.Visibility = Visibility.Hidden
        img2.Visibility = Visibility.Hidden
        img3.Visibility = Visibility.Hidden
        img4.Visibility = Visibility.Hidden
    End Sub


    '=============================Seg 설정 됬을때 - Drop
    Private Sub SegCol_Drop(sender As Object, e As DragEventArgs) Handles SegCol.Drop
        If RB04.IsChecked Then
            MsgBox("선행 Seg를 상속 받은 경우에는 Segment 내용을 변경 할 수 없습니다.", MsgBoxStyle.Information, "D³F Python")
        Else
            Dim DragObj As objectPRD = e.Data.GetData(GetType(objectPRD))
            If Not DragObj Is Nothing Then
                If DragObj.Type_P.Contains("Field") Then
                    SegCol.ViewerPanel.Children.Clear()
                    Dim Obj As objectPRD = New objectPRD With {.Type_P = DragObj.Type_P,
                                                                .Name_P = DragObj.Name_P,
                                                                .Alias_P = DragObj.Alias_P,
                                                                .KIND_P = DragObj.KIND_P,
                                                                .ExReVisible = DragObj.ExReVisible,
                                                                .isSub = DragObj.isSub,
                                                                .SetID = DragObj.SetID,
                                                                .MasterName_P = DragObj.MasterName_P,
                                                                .SetMasterID = DragObj.SetMasterID}

                    SegCol.ViewerPanel.Children.Add(Obj)
                    If (Not DragObj.KIND_P.Equals("PY")) Then ToggleMSObject(Obj.SetMasterID, "M")

                    'Add SegValue object
                    InputList.AddObject("String", "SEG_IDX", "SEG_IDX", 999, "PY",,,, True)
                    InputList.AddObject("String", "SEG_VAL", "SEG_VAL", 998, "PY",,,, True)

                    SegColName = DragObj.Name_P
                    SegTBLName = DragObj.MasterName_P
                    SegTBLKind = DragObj.KIND_P

                    RaiseEvent SegDrop(Me, "SegDrop")
                End If
            End If
        End If
    End Sub
    '=============================Seg 설정 됬을때 - 외부
    'typeΟNameΟAliasΟIsSub
    Public Sub SetSegColInfo(SegInfo As String)
        If String.IsNullOrEmpty(SegInfo) = False Then
            SegCol.ViewerPanel.Children.Clear()
            Dim Type, Name, _Alias, IsSub, MasterName, MasterID, Kind As String

            Type = SegInfo.Split("Ο")(0)
            Name = SegInfo.Split("Ο")(1)
            _Alias = SegInfo.Split("Ο")(2)
            IsSub = SegInfo.Split("Ο")(3)
            MasterName = SegInfo.Split("Ο")(4)
            MasterID = SegInfo.Split("Ο")(5)
            If (SegInfo.Split("Ο").Length = 7) Then
                Kind = SegInfo.Split("Ο")(6)
            Else
                Kind = "DS"
            End If


            SegColName = Name
            SegTBLName = MasterName
            SegTBLKind = Kind

            '사라진 Column이 아닌지 확인
            For Each ele As UIElement In SegColList.ViewerPanel.Children
                Dim obj As objectPRD = TryCast(ele, objectPRD)
                If obj IsNot Nothing AndAlso (obj.Type_P.Equals(Type) And obj.Name_P.Equals(Name) And obj.Alias_P.Equals(_Alias)) Then
                    SegCol.AddObject(Type, Name, _Alias, obj.SetID, Kind, IsSub, MasterName, obj.SetMasterID)
                    If (Not obj.KIND_P.Equals("PY")) Then ToggleMSObject(obj.SetMasterID, "M")
                    'Add SegValue object

                    InputList.AddObject("String", "SEG_IDX", "SEG_IDX", 999, "PY",,,, True)
                    InputList.AddObject("String", "SEG_VAL", "SEG_VAL", 998, "PY",,,, True)
                    Exit For
                End If
            Next
        End If
    End Sub

    '==========================Master table관련 모두 Multi로 전환 / Single로 전환
    Public Sub ToggleMSObject(MaterID As Integer, Type As String)
        For Each obj As Object In InputList.ViewerPanel.Children
            Dim realObj As objectPRD = TryCast(obj, objectPRD)
            If Not realObj Is Nothing Then
                If realObj.SetID = MaterID Then     'Table
                    If Type.Equals("M") Then
                        If realObj.Type_P.Equals("DataSet_S") Then
                            realObj.Name_P = realObj.Name_P.Replace("_{?}", "") & "_{?}"
                            realObj.Alias_P = realObj.Alias_P.Replace("_{?}", "") & "_{?}"
                        End If
                        realObj.Type_P = "DataSet_M"
                    ElseIf Type.Equals("S") Then
                        If realObj.Type_P.Equals("DataSet_M") Then
                            realObj.Name_P = realObj.Name_P.Replace("_{?}", "")
                            realObj.Alias_P = realObj.Alias_P.Replace("_{?}", "")
                        End If
                        realObj.Type_P = "DataSet_S"
                    End If
                ElseIf realObj.SetMasterID = MaterID Then   'Field
                    If Type.Equals("M") Then
                        If realObj.MasterName_P.Contains("_{?}") = False Then
                            realObj.MasterName_P = realObj.MasterName_P.Replace("_{?}", "") & "_{?}"
                        End If
                    ElseIf Type.Equals("S") Then
                        realObj.MasterName_P = realObj.MasterName_P.Replace("_{?}", "")
                    End If
                Else    '나머지는 Single
                    'realObj.Name_P = realObj.Name_P.Replace("_{?}", "")
                    'realObj.Alias_P = realObj.Alias_P.Replace("_{?}", "")
                End If
            End If
        Next

    End Sub

    '==========================각각의 DataSet수행일 경우 ViewerPanel DataSet List 변환
    Public Sub ToggleEachDataSet()
        InputList.ViewerPanel.Children.Clear()
        InputList.AddObject("DataSet_M", "{Input DataSet}", "{Input DataSet}", 1, "DS")
    End Sub


    Public Function GetSegColInfo()
        Dim SegColInfo As String = ""
        Dim ObjPrd As objectPRD = TryCast(SegCol.ViewerPanel.Children(0), objectPRD)
        If Not ObjPrd Is Nothing Then
            SegColInfo = ObjPrd.Type_P & "Ο" & ObjPrd.Name_P & "Ο" & ObjPrd.Alias_P & "Ο" & ObjPrd.isSub & "Ο" & ObjPrd.MasterName_P & "Ο" & ObjPrd.SetMasterID & "Ο" & ObjPrd.KIND_P
        End If

        Return SegColInfo
    End Function

    Public Sub SetSegText(SegData As String)
        If String.IsNullOrEmpty(SegData) Then
            Dim obj As objectPRD = TryCast(SegCol.ViewerPanel.Children(0), objectPRD)
            If obj IsNot Nothing Then
                MsgBox(obj.Name_P & "의 Data가 없어 Segment로 사용할 수 없습니다.", MsgBoxStyle.Information, "D³F Python")
                SegCol.ViewerPanel.Children.Clear()
                SegCol.ViewerPanel.Children.Add(New Label With {.Height = 30, .Content = "Drop the Column Here", .VerticalContentAlignment = VerticalAlignment.Center,
                                                .HorizontalContentAlignment = HorizontalAlignment.Center, .Opacity = 0.2, .Background = Brushes.Transparent})
                SegText.Clear()
            End If

        Else
            SegText.Text = SegData
        End If
    End Sub

    Public Function GetSegText() As String
        Return SegText.Text
    End Function

    Public Sub ExRe_MouseDown(sender As objectPRD, e As Boolean)

        For Each obj As objectPRD In InputList.ViewerPanel.Children
            If obj.MasterName_P.Equals(sender.Name_P) And obj.SetMasterID = sender.SetID Then
                Dim idx As Integer = InputList.ViewerPanel.Children.IndexOf(obj)
                If e Then
                    InputList.ViewerPanel.Children(idx).Visibility = Visibility.Visible
                Else
                    InputList.ViewerPanel.Children(idx).Visibility = Visibility.Collapsed
                End If

            End If
        Next
    End Sub

    Public Sub ExRe_MouseDown_Seg(sender As objectPRD, e As Boolean)

        For Each obj As objectPRD In SegColList.ViewerPanel.Children
            If obj.MasterName_P.Equals(sender.Name_P) And obj.SetMasterID = sender.SetID Then
                Dim idx As Integer = SegColList.ViewerPanel.Children.IndexOf(obj)
                If e Then
                    SegColList.ViewerPanel.Children(idx).Visibility = Visibility.Visible
                Else
                    SegColList.ViewerPanel.Children(idx).Visibility = Visibility.Collapsed
                End If

            End If
        Next
    End Sub

    Public Property PY_Name() As String
        Get
            Return txt_NM.Text
        End Get
        Set(value As String)
            txt_NM.Text = value
        End Set
    End Property

    Public Property PY_Desc() As String
        Get
            Return txt_Desc.Text
        End Get
        Set(value As String)
            txt_Desc.Text = value
        End Set
    End Property

    Public Property PY_ExWay() As String
        Get
            Dim ExWayCode As String = ""
            For Each rdo As RadioButton In rdo_ExWay.Children
                If rdo.IsChecked Then
                    ExWayCode = rdo.Tag.ToString
                    Exit For
                End If
            Next
            Return ExWayCode
        End Get
        Set(value As String)
            For Each rdo As RadioButton In rdo_ExWay.Children
                If rdo.Tag.Equals(value) Then
                    rdo.IsChecked = True
                    Exit For
                End If
            Next
        End Set
    End Property

    Public Property PY_ExType() As String
        Get
            Dim ExTypeCode As String = ""
            For Each rdo As RadioButton In rdo_ExType.Children
                If rdo.IsChecked Then
                    ExTypeCode = rdo.Tag.ToString
                    Exit For
                End If
            Next
            Return ExTypeCode
        End Get
        Set(value As String)
            For Each rdo As RadioButton In rdo_ExType.Children
                If rdo.Tag.Equals(value) Then
                    rdo.IsChecked = True
                    Exit For
                End If
            Next
        End Set
    End Property

    'InputList : typeΟNameΟAliasΟIsSubΘtypeΟNameΟAliasΟIsSub
    'Type : DataSet_M, DataSet_S, Field_Str, Field_Int, Model, Etc, String
    'IsSub : 1, 0
    Public Sub SetInput(InputListPara As String)
        If (String.IsNullOrEmpty(InputListPara)) Then
            'Input is nothing so, hide input list

            ''Design_InputArea.Width = New GridLength(0)
            ''Coding_InputArea.Width = New GridLength(0)
            ''Coding_barArea.Width = New GridLength(0)
        Else
            InputListPara_backup = InputListPara
            Dim InputArray() As String = InputListPara.Split("Θ")
            Dim Type, Name, _Alias, IsSub, isPYDS As String

            InputViewer.ViewerPanel.Children.Clear()
            InputList.ViewerPanel.Children.Clear()
            SegColList.ViewerPanel.Children.Clear()

            If (String.IsNullOrEmpty(InputListPara) = False) Then
                SegColList.IsSegment = True
                SingleCnt = 0
                MultiCnt = 0
                'MasterSub.Clear()
                Dim ID As Integer = 1
                Dim MasterNM As String = ""
                Dim MasterType As String = ""
                Dim MasterID As Integer = 0

                For Each input As String In InputArray
                    Type = input.Split("Ο")(0)
                    Name = input.Split("Ο")(1)
                    _Alias = input.Split("Ο")(2)
                    IsSub = input.Split("Ο")(3)
                    isPYDS = input.Split("Ο")(4)

                    If (String.IsNullOrEmpty(input.Trim) = False) Then
                        If Type.Contains("DataSet") Then
                            MasterType = Type

                            InputViewer.AddObject(Type, Name, _Alias, ID, isPYDS)           'Design
                            InputList.AddObject(Type, Name, _Alias, ID, isPYDS, IsSub)      'Coding
                            If Type.Equals("DataSet_M") Then
                                MultiCnt += 1
                            ElseIf Type.Equals("DataSet_S") Then
                                SingleCnt += 1
                            End If

                            If IsSub.Equals("1") Then
                                MasterNM = Name
                                MasterID = ID
                                'MasterSub.Add(Name, New List(Of String))
                                If Type.Equals("DataSet_S") Then SegColList.AddObject(Type, Name, _Alias, ID, isPYDS, IsSub) 'Segment   'And isPYDS.Equals("DS") - PY도 Seg가능
                            Else
                                MasterNM = ""
                                MasterID = 0
                            End If
                        ElseIf Type.Contains("Field") Then
                            InputList.AddObject(Type, Name, _Alias, ID, isPYDS, IsSub, MasterNM, MasterID)
                            If MasterType.Equals("DataSet_S") Then SegColList.AddObject(Type, Name, _Alias, ID, isPYDS, IsSub, MasterNM, MasterID) 'Segment    'And isPYDS.Equals("DS") - PY도 Seg가능
                            'If String.IsNullOrEmpty(MasterNM) = False Then
                            '   MasterSub(MasterNM).Add(Name)
                            'End If

                            If Type.Equals("Field_Str") Then
                                StringCnt += 1
                            ElseIf Type.Equals("Field_Int") Then
                                IntegerCnt += 1
                            End If
                        ElseIf Type.Contains("Model") Then
                            InputViewer.AddObject(Type, Name, _Alias, ID, isPYDS)
                            InputList.AddObject(Type, Name, _Alias, ID, isPYDS, IsSub)
                            ModelCnt += 1
                        ElseIf Type.Contains("Etc") Then
                            InputViewer.AddObject(Type, Name, _Alias, ID, isPYDS)
                            InputList.AddObject(Type, Name, _Alias, ID, isPYDS, IsSub)
                            ETCCnt += 1
                        ElseIf Type.Contains("String") Then
                            InputList.AddObject(Type, Name, _Alias, ID, isPYDS, IsSub)
                        End If

                    End If

                    ID += 1
                Next
            End If
        End If
    End Sub

    Public Function GetOutput() As String
        Dim OutputData As String = ""
        Dim OutputDupChk As New List(Of String)
        Dim Dup As Boolean = False
        For Each txtbox As TextBox In Output_Data.Children
            If (Not String.IsNullOrEmpty(txtbox.Text)) Then
                OutputData &= "ΘDataSetΟ" & txtbox.Text
                If (OutputDupChk.Contains(txtbox.Text)) Then
                    MsgBox("Output 변수명이 중복됩니다." & vbNewLine & "변수 명 : " & txtbox.Text, MsgBoxStyle.Critical, "D³F Python")
                    Dup = True
                    Return "ERR"
                ElseIf txtbox.Text.Contains("InputFile") Then
                    MsgBox("DataFrame의 이름에 'InputFile'단어는 사용할 수 없습니다." & vbNewLine & "변수 명 : " & txtbox.Text, MsgBoxStyle.Critical, "D³F Python")
                    Dup = True
                    Return "ERR"
                Else
                    OutputDupChk.Add(txtbox.Text)
                End If
            End If
        Next

        For Each txtbox As TextBox In Output_Model.Children
            If (Not String.IsNullOrEmpty(txtbox.Text)) Then
                OutputData &= "ΘModelΟ" & txtbox.Text
                If (OutputDupChk.Contains(txtbox.Text)) Then
                    MsgBox("Output 변수명이 중복됩니다." & vbNewLine & "변수 명 : " & txtbox.Text, MsgBoxStyle.Critical, "D³F Python")
                    Dup = True
                    Return "ERR"
                Else
                    OutputDupChk.Add(txtbox.Text)
                End If
            End If
        Next

        For Each txtbox As TextBox In Output_etc.Children
            If (Not String.IsNullOrEmpty(txtbox.Text)) Then
                OutputData &= "ΘEtcΟ" & txtbox.Text
                If (OutputDupChk.Contains(txtbox.Text)) Then
                    MsgBox("Output 변수명이 중복됩니다." & vbNewLine & "변수 명 : " & txtbox.Text, MsgBoxStyle.Critical, "D³F Python")
                    Dup = True
                    Return "ERR"
                Else
                    OutputDupChk.Add(txtbox.Text)
                End If
            End If
        Next

        For Each txtbox As TextBox In Output_Image.Children
            If (Not String.IsNullOrEmpty(txtbox.Text)) Then
                OutputData &= "ΘImageΟ" & txtbox.Text
                If (OutputDupChk.Contains(txtbox.Text)) Then
                    MsgBox("Output 변수명이 중복됩니다." & vbNewLine & "변수 명 : " & txtbox.Text, MsgBoxStyle.Critical, "D³F Python")
                    Dup = True
                    Return "ERR"
                Else
                    OutputDupChk.Add(txtbox.Text)
                End If
            End If
        Next

        If OutputData.Length > 0 Then OutputData = OutputData.Substring(1)

        Return OutputData
    End Function

    'OutputList : typeΟaaaΘtypeΟbbb
    'Type : DataSet, Model, Etc
    Public Sub SetOutput(OutputPara As String)
        Output_Data.Children.Clear()
        Output_Model.Children.Clear()
        Output_etc.Children.Clear()

        Dim OutputList As String() = OutputPara.Split("Θ")
        Dim Type As String, Text As String
        For Each output As String In OutputList
            If Not String.IsNullOrEmpty(output) Then
                Type = output.Split("Ο")(0)
                Text = output.Split("Ο")(1)

                Dim TxtBox As TextBox = New TextBox With {.Text = Text, .Margin = New Thickness(3), .MinWidth = 100}
                AddHandler TxtBox.LostFocus, AddressOf TxtLostFocus

                If Type.Equals("DataSet") Then
                    Output_Data.Children.Add(TxtBox)
                ElseIf Type.Equals("Model") Then
                    Output_Model.Children.Add(TxtBox)
                ElseIf Type.Equals("Etc") Then
                    Output_etc.Children.Add(TxtBox)
                ElseIf Type.Equals("Image") Then
                    Output_Image.Children.Add(TxtBox)
                End If
            End If
        Next
    End Sub
    Private Sub Image_MouseDown(sender As Object, e As MouseButtonEventArgs)    'DataSet
        Dim TxtBox As TextBox = New TextBox With {.Margin = New Thickness(3), .MinWidth = 100}
        AddHandler TxtBox.LostFocus, AddressOf TxtLostFocus
        Output_Data.Children.Add(TxtBox)
    End Sub

    Private Sub Image_MouseDown_1(sender As Object, e As MouseButtonEventArgs)  'Model
        Dim TxtBox As TextBox = New TextBox With {.Margin = New Thickness(3), .MinWidth = 100}
        AddHandler TxtBox.LostFocus, AddressOf TxtLostFocus
        Output_Model.Children.Add(TxtBox)
    End Sub

    Private Sub Image_MouseDown_2(sender As Object, e As MouseButtonEventArgs)  'etc
        Dim TxtBox As TextBox = New TextBox With {.Margin = New Thickness(3), .MinWidth = 100}
        AddHandler TxtBox.LostFocus, AddressOf TxtLostFocus
        Output_etc.Children.Add(TxtBox)
    End Sub

    Private Sub Image_MouseDown_3(sender As Object, e As MouseButtonEventArgs)
        Dim TxtBox As TextBox = New TextBox With {.Margin = New Thickness(3), .MinWidth = 100}
        AddHandler TxtBox.LostFocus, AddressOf TxtLostFocus
        Output_Image.Children.Add(TxtBox)
    End Sub

    Public Sub SetCode(Code As String)
        Editor.Code = Code
        If (Not String.IsNullOrEmpty(Code)) Then
            OnloadBool = True
            MasterTab.SelectedItem = Coding
        End If
    End Sub

    Public Function GetCode() As String
        Return Editor.Code
    End Function

    Public Sub SetCode_Header(Code As String)
        Editor_header.Code = Code
    End Sub

    Public Function GetCode_Header() As String
        Return Editor_header.Code
    End Function

    Public Sub TxtLostFocus(sender As TextBox, e As RoutedEventArgs)
        If String.IsNullOrEmpty(sender.Text) Then
            CType(sender.Parent, WrapPanel).Children.Remove(sender)
        End If
    End Sub

    Private Sub CheckBox_Click(sender As CheckBox, e As RoutedEventArgs)
        For Each obj As objectPRD In InputList.ViewerPanel.Children
            obj.ViewName = sender.IsChecked
        Next
    End Sub
    Private Sub CheckBox_Click_1(sender As Object, e As RoutedEventArgs)
        For Each obj As objectPRD In SegColList.ViewerPanel.Children
            obj.ViewName = sender.IsChecked
        Next
    End Sub
    Private Sub Output_Data_Drop(sender As WrapPanel, e As DragEventArgs) Handles Output_Data.Drop
        Dim TxtBox As TextBox = New TextBox With {.Margin = New Thickness(3), .MinWidth = 100, .Text = e.Data.GetData("System.String")}
        AddHandler TxtBox.LostFocus, AddressOf TxtLostFocus
        sender.Children.Add(TxtBox)
    End Sub

    Private Sub Output_Model_Drop(sender As WrapPanel, e As DragEventArgs) Handles Output_Model.Drop
        Dim TxtBox As TextBox = New TextBox With {.Margin = New Thickness(3), .MinWidth = 100, .Text = e.Data.GetData("System.String")}
        AddHandler TxtBox.LostFocus, AddressOf TxtLostFocus
        sender.Children.Add(TxtBox)
    End Sub

    Private Sub Output_etc_Drop(sender As WrapPanel, e As DragEventArgs) Handles Output_etc.Drop
        Dim TxtBox As TextBox = New TextBox With {.Margin = New Thickness(3), .MinWidth = 100, .Text = e.Data.GetData("System.String")}
        AddHandler TxtBox.LostFocus, AddressOf TxtLostFocus
        sender.Children.Add(TxtBox)
    End Sub

    Private Sub Output_Image_Drop(sender As WrapPanel, e As DragEventArgs) Handles Output_Image.Drop
        Dim TxtBox As TextBox = New TextBox With {.Margin = New Thickness(3), .MinWidth = 100, .Text = e.Data.GetData("System.String")}
        AddHandler TxtBox.LostFocus, AddressOf TxtLostFocus
        sender.Children.Add(TxtBox)
    End Sub

    'Tab Change
    Private Sub TabControl_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If OnloadBool Then
            OnloadBool = False
        Else
            If Design.IsSelected Then

            ElseIf Segment.IsSelected Then

            ElseIf Coding.IsSelected Then
                If (PY_ExWay.Equals("02") Or PY_ExWay.Equals("04")) And String.IsNullOrEmpty(GetSegColInfo) Then
                    MsgBox("Segment 설정을 해주세요.", MsgBoxStyle.Critical, "D³F Python")
                    Segment.IsSelected = True
                End If
            ElseIf Result.IsSelected Then

            End If
        End If
    End Sub

    Public Event ExWaySelect(SegColInfo As Object, EventName As String)
    Public SelWayCode As String = ""
    Private Sub RadioButton_Checked(sender As RadioButton, e As RoutedEventArgs)
        Try
            If sender.Name.Equals("RB02") Or sender.Name.Equals("RB04") Then
                Segment.Visibility = Visibility.Visible
            ElseIf sender.Name.Equals("RB03") Then
                ToggleEachDataSet()
                InputList.RemoveObject("SEG_IDX")
                InputList.RemoveObject("SEG_VAL")
            Else
                InputList.RemoveObject("SEG_IDX")
                InputList.RemoveObject("SEG_VAL")
            End If
            SelWayCode = sender.Name.Substring(2)
            RaiseEvent ExWaySelect(Me, "ExWaySelect")
        Catch ex As Exception

        End Try

    End Sub

    Private Sub RadioButton_Unchecked(sender As RadioButton, e As RoutedEventArgs)

        If sender.Name.Equals("RB02") Or sender.Name.Equals("RB04") Then
            Segment.Visibility = Visibility.Collapsed

            For Each obj As Object In SegCol.ViewerPanel.Children()
                Dim realobj As objectPRD = TryCast(obj, objectPRD)
                If Not realobj Is Nothing Then
                    ToggleMSObject(realobj.SetMasterID, "S")
                End If
            Next
        ElseIf sender.Name.Equals("RB03") Then
            SetInput(InputListPara_backup)
        End If
    End Sub

    Public Sub SetOnloadPossibleWay(PossibleWay As String)
        Dim Waylist() As String = PossibleWay.Split(";")
        Dim CheckRB As New List(Of RadioButton)
        For Each Way As String In Waylist
            If String.IsNullOrEmpty(Way) = False Then
                If (Way.Equals("01")) Then
                    RB01.IsEnabled = True
                    RB01.Opacity = 1
                    CheckRB.Add(RB01)
                ElseIf (Way.Equals("02")) Then
                    RB02.IsEnabled = True
                    RB02.Opacity = 1
                    CheckRB.Add(RB02)
                ElseIf (Way.Equals("03")) Then
                    RB03.IsEnabled = True
                    RB03.Opacity = 1
                    CheckRB.Add(RB03)
                ElseIf (Way.Equals("04")) Then
                    RB04.IsEnabled = True
                    RB04.Opacity = 1
                    CheckRB.Add(RB04)
                End If
            End If
        Next
        If CheckRB.Count > 0 Then
            CheckRB(0).IsChecked = True
        End If
    End Sub

    '================================Result
    Public Sub SetResultTxt(outTxt As String, ErrTxt As String)
        Txt_Output.Text = outTxt

        If ErrTxt.Equals("CODE") Then
            MasterTab.SelectedItem = Coding
        Else
            Txt_Error.Text = ErrTxt
            If String.IsNullOrEmpty(ErrTxt) = False Then
                OutTxtTab.SelectedIndex = 1
            End If

            If Not String.IsNullOrEmpty(outTxt) Or Not String.IsNullOrEmpty(ErrTxt) Then
                MasterTab.SelectedItem = Result
            End If
        End If

    End Sub

    Public Sub SetResultDataTab(ReultDataInfo As String)   'HeaderΟDataΘHeaderΟDataΘHeaderΟData
        Dim DataList() As String = ReultDataInfo.Split("Θ")
        Dim Cnt As Integer = 0
        DataViewTab.Items.Clear()

        For Each DataInfo As String In DataList
            If String.IsNullOrEmpty(DataInfo) = False Then
                Cnt += 1
                Dim Info() As String = DataInfo.Split("Ο")

                Dim newTxt As New TextBox With {.AcceptsReturn = True, .VerticalScrollBarVisibility = ScrollBarVisibility.Auto, .HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, .IsReadOnly = True, .Text = Info(1)}
                Dim newTab As New TabItem
                newTab.Header = Info(0)
                newTab.Content = newTxt

                DataViewTab.Items.Add(newTab)
            End If
        Next

        DataViewTab.SelectedIndex = 0
    End Sub

    Public Sub SetResultImageTab(ReultDataInfo As String)   'HeaderΟDataΘHeaderΟDataΘHeaderΟData
        Dim DataList() As String = ReultDataInfo.Split("Θ")
        Dim Cnt As Integer = 0
        ImageTab.Items.Clear()

        For Each DataInfo As String In DataList
            If String.IsNullOrEmpty(DataInfo) = False Then
                Cnt += 1
                Dim Info() As String = DataInfo.Split("Ο")

                Dim imageArr As Byte() = System.Text.Encoding.UTF8.GetBytes(Info(1))
                Dim bi As BitmapImage = New BitmapImage()
                bi.BeginInit()
                bi.CreateOptions = BitmapCreateOptions.None
                bi.CacheOption = BitmapCacheOption.Default
                bi.StreamSource = New MemoryStream(imageArr)
                bi.EndInit()

                Dim img As New Image
                img.Source = bi

                Dim newTab As New TabItem
                newTab.Header = Info(0)
                newTab.Content = img

                ImageTab.Items.Add(newTab)
            End If
        Next

        If Cnt = 0 Then
            ImagePlace.Width = New GridLength(0, GridUnitType.Pixel)
            Splitter.Width = New GridLength(0, GridUnitType.Pixel)
        Else
            ImagePlace.Width = New GridLength(1, GridUnitType.Star)
            Splitter.Width = New GridLength(5, GridUnitType.Pixel)
        End If

        ImageTab.SelectedIndex = 0
    End Sub

    Public Sub SetResultSegList(SegList As String)   'SegIDΟSegNMΘSegIDΟSegNMΘSegIDΟSegNM
        If (String.IsNullOrEmpty(SegList)) Then
            SegPlace.Width = New GridLength(0, GridUnitType.Pixel)
            Segsplitter.Width = New GridLength(0, GridUnitType.Pixel)
        Else
            SegPlace.Width = New GridLength(1, GridUnitType.Star)
            Segsplitter.Width = New GridLength(5, GridUnitType.Pixel)

            Dim DataList() As String = SegList.Split("Θ")
            SegPanel.Children.Clear()

            Dim idx As Integer = 0
            For Each Seg As String In DataList
                Dim Info() As String = Seg.Split("Ο")

                Dim newSeg As New SegObject
                newSeg.SegID = Info(0)
                newSeg.SegName = Info(1)
                newSeg.Cursor = Cursors.Hand
                Dim bnd As New Binding("ActualWidth") With {.ElementName = "SegPanel"}
                BindingOperations.SetBinding(newSeg, SegObject.WidthProperty, bnd)
                AddHandler newSeg.SegButton.Click, AddressOf SegButtonClick

                If (idx = 0) Then
                    newSeg.SelectedYN(True)
                Else
                    newSeg.SelectedYN(False)
                End If

                SegPanel.Children.Add(newSeg)

                idx += 1
            Next
        End If
    End Sub

    Public SelSegID As String
    Public Event SegResultClick(SegColInfo As Object, EventName As String)
    Public Sub SegButtonClick(sender As Button, e As RoutedEventArgs)

        For Each SegObj As SegObject In SegPanel.Children
            If SegObj.SegID.Equals(sender.Tag) Then
                SegObj.SelectedYN(True)
            Else
                SegObj.SelectedYN(False)
            End If
        Next

        SelSegID = sender.Tag
        RaiseEvent SegResultClick(Me, "SegResultClick")
    End Sub

    Public Event ResultRefresh(SegColInfo As Object, EventName As String)
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        RaiseEvent ResultRefresh(Me, "ResultRefresh")
    End Sub


    Public Event SelInputFile(SegColInfo As Object, EventName As String)
    'Add Input File event
    Private Sub AddInputFile_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles AddInputFile.MouseDown
        RaiseEvent SelInputFile(Me, "SelInputFile")
    End Sub

    Public Event ShowPythonExCode(SegColInfo As Object, EventName As String)
    Private Sub PythonExCode_Click(sender As Object, e As RoutedEventArgs) Handles PythonExCode.Click
        RaiseEvent ShowPythonExCode(Me, "ShowPythonExCode")
    End Sub
End Class