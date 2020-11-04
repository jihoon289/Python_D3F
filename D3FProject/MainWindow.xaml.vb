Class MainWindow
    Public Sub New()

        ' 디자이너에서 이 호출이 필요합니다.
        InitializeComponent()

        ' InitializeComponent() 호출 뒤에 초기화 코드를 추가하세요.

    End Sub

    Private Sub Btn1_Click(sender As Object, e As RoutedEventArgs) Handles btn1.Click
        python.ScreenMode()

        python.SetInput("DataSet_MΟMulti DataSetΟMulti DataSet AliasΟ0ΟDSΘDataSet_SΟSingle DataSetΟSingle DataSet AliasΟ1ΟDSΘField_StrΟString FieldΟString Field AliasΟ0ΟDSΘField_IntΟInteger FieldΟInteger Field AliasΟ0ΟDSΘ" &
                        "DataSet_MΟMulti DataSetΟMulti DataSet AliasΟ0ΟDSΘDataSet_SΟSingle DataSetΟSingle DataSet AliasΟ1ΟDSΘField_StrΟString FieldΟString Field AliasΟ0ΟDSΘ" &
                        "ModelΟModelΟModel AliasΟ0ΟDAΘEtcΟETCΟETCΟ0ΟDS")

        python.SetOutput("DataSetΟAAAΘDataSetΟBBBΘModelΟModel_AΘModelΟModel_BΘEtcΟEtc_AΘEtcΟEtc_B")

        Dim Code As String = "import matplotlib" & vbNewLine &
                            "import numpy as np" & vbNewLine &
                            "import matplotlib.pyplot as plt" & vbNewLine &
                            "%matplotlib inline  " & vbNewLine &
                            "" & vbNewLine &
                            "# set graph image size" & vbNewLine &
                            "fig = plt.figure(figsize=(8, 6), dpi=80)" & vbNewLine &
                            "" & vbNewLine &
                            "xs = np.linspace(0.0, 10.0, 1000)" & vbNewLine &
                            "ys = [x * x for x in xs]" & vbNewLine &
                            "zs = [x * x * x for x in xs]" & vbNewLine &
                            "" & vbNewLine &
                            "# set range" & vbNewLine &
                            "# axes = plt.gca()" & vbNewLine &
                            "# axes.set_xlim([-0.1, -2.0])" & vbNewLine &
                            "" & vbNewLine &
                            "plt.plot(xs, ys)" & vbNewLine &
                            "plt.plot(xs, zs)" & vbNewLine &
                            "plt.title('A simple graph')" & vbNewLine &
                            "plt.legend(['y = x * x', 'y = x * x * x'], loc='upper left')" & vbNewLine &
                            "plt.show()"

        python.SetCode(Code)

        python.SetOnloadPossibleWay("02;03;04")
    End Sub
End Class
