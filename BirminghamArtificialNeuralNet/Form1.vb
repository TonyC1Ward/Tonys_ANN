Imports System.IO
Imports System.Math
Imports BirminghamArtificialNeuralNet.GlobalVariables

Public Class BhamANN

    Private Sub Button1_Click(sender As Object, e As EventArgs)
        TargetValues = LoadDataArray(TargetValues)
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs)
        TrainingValues = LoadDataArray(TrainingValues)
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs)
        Dim i, j As Int16
        Dim TheData As String
        TheData = ""

        For i = 0 To TrainingValues.GetUpperBound(0) - 1
            For j = 0 To TrainingValues.GetUpperBound(1) - 1
                TheData = TheData + CStr(TrainingValues(i, j))
            Next
            MsgBox(i & " " & "Data is >" & " " & TheData)
            TheData = ""
        Next
    End Sub

    Private Function LoadDataArray(ByVal ArrayToLoad(,) As Double) As Double(,)
        Dim FileNameNeeded As String
        FileNameNeeded = ""
        If (OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK) Then
            FileNameNeeded = OpenFileDialog1.FileName
        End If
        Dim reader As New StreamReader(FileNameNeeded)
        Dim TheNextLine As String
        Dim i, j As Integer
        i = 0
        j = 0
        Do
            TheNextLine = reader.ReadLine
            If TheNextLine Is Nothing Then
                Exit Do
            End If
            Dim Elements As String() = TheNextLine.Split(",")
            j = 0
            For Each Element In Elements
                ArrayToLoad(i, j) = CDbl(Element)
                j = j + 1
            Next
            i = i + 1
        Loop Until TheNextLine Is Nothing
        reader.Close()
        Return ArrayToLoad
        'MsgBox("Target set loaded")
    End Function

    Private Sub BhamANN_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitialiseWeightInputToHidden()
        InitialiseWeightHiddenToOutput()
        DataGridView5.Columns("equals").ValueType = System.Type.GetType("System.String")
        DataGridView5.Columns("Multiply1").ValueType = System.Type.GetType("System.String")
        DataGridView5.Columns("Multiply2").ValueType = System.Type.GetType("System.String")
        DataGridView5.Columns("Add1").ValueType = System.Type.GetType("System.String")
        DataGridView5.Columns("Multiply3").ValueType = System.Type.GetType("System.String")
        DataGridView5.Columns("equals").Width = 30
        DataGridView5.Columns("Multiply1").Width = 45
        DataGridView5.Columns("Multiply2").Width = 45
        DataGridView5.Columns("Multiply3").Width = 45
        DataGridView5.Columns("Add1").Width = 30
        Chart1.Series.Add("Error")
        With Chart1.Series("Error")
            .ChartType = DataVisualization.Charting.SeriesChartType.Line
            .Color = Color.Red
            '.Points.AddXY(0, 100)
            '.Points.AddXY(1, 90)
            '.Points.AddXY(2, 80)
            '.Points.AddXY(3, 60)
            '.Points.AddXY(4, 30)
        End With

    End Sub


    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        TrainTheNeuralNetwork()
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Dim i As Int16
        DataGridView2.Columns(0).DefaultCellStyle.BackColor = Color.DarkGray
        TargetValues = LoadDataArray(TargetValues)
        For i = 1 To 4
            DataGridView2.Rows.Add(New String() {i, TargetValues(i, 1)})
        Next

    End Sub

    Private Sub Button3_Click_1(sender As Object, e As EventArgs) Handles Button3.Click
        Dim i As Int16
        Dim OrValue As Int16 = 0

        TrainingValues = LoadDataArray(TrainingValues)
        DataGridView3.Columns(0).DefaultCellStyle.BackColor = Color.DarkGray
        For i = 1 To 4
            If TrainingValues(i, 1) Or TrainingValues(i, 2) = 1 Then
                OrValue = 1
            Else
                OrValue = 0
            End If
            DataGridView3.Rows.Add(New String() {i, TrainingValues(i, 1), TrainingValues(i, 2), OrValue})
        Next

    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs)
        Dim tony As Int16
        Dim ward As Int16
        Dim TheResult As String = ""
        For tony = 1 To 4
            For ward = 1 To 2
                TheResult = TheResult + CStr(Round(Output(tony, ward)))
            Next
            TheResult = TheResult + vbCrLf
        Next
        MsgBox(TheResult)
    End Sub

    Private Sub Button2_Click_2(sender As Object, e As EventArgs) Handles Button2.Click
        Dim i As Int16
        DataGridView4.Columns(0).DefaultCellStyle.BackColor = Color.DarkGray
        For i = 1 To 4
            DataGridView4.Rows.Add(New String() {i, Output(i, 1), Round(Output(i, 1))})
        Next
    End Sub

    Private Sub Button4_Click_1(sender As Object, e As EventArgs) Handles Button4.Click
        Dim Accum As Double
        Dim TestInput(NumberOfInputNodes) As Double
        TestInput(0) = 0.0
        TestInput(1) = 0.0
        TestInput(2) = 0.0


        For j = 1 To NumberOfHiddenNodes
            Accum = WeightInputToHidden(NumberOfInputNodes, j)
            For i = 1 To NumberOfInputNodes
                Accum += TestInput(j - 1) * WeightInputToHidden(i, j)
                MsgBox(CStr(Accum))
            Next
            Hidden(1, j) = ANNMainFunctions.Sigmoid(Accum)
        Next

        For k = 1 To NumberOfOutputNodes   ' compute output unit activations And errors */
            Accum = WeightHiddenToOutput(NumberOfHiddenNodes, k)
            For j = 1 To NumberOfHiddenNodes
                Accum += Hidden(j, k) * WeightHiddenToOutput(j, k)
            Next
            Output(1, k) = ANNMainFunctions.Sigmoid(Accum)
        Next


    End Sub
End Class
