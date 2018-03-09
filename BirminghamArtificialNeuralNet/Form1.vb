Imports System.IO
Imports System.Math

Public Class BhamANN
    Public Const NumberOfInputNodes As Int16 = 2
    Public Const NumberOfHiddenNodes As Int16 = 2
    Public Const smallwt As Double = 0.5
    Public Const NumberOfOutputNodes As Int16 = 1
    Public Const NumberOfTrainingRecords As Int16 = 4
    Public Const SUCCESS As Double = 0.0002
    Public Const MAX_EPOCH As Int64 = 2000000
    Public Const NumberOfTrainingFields As Int64 = 3


    Public DeltaWeightInputToHidden(NumberOfInputNodes + 1, NumberOfHiddenNodes + 1) As Double
    Public WeightInputToHidden(NumberOfInputNodes + 1, NumberOfHiddenNodes + 1) As Double
    Public DeltaWeightHiddenToOutput(NumberOfHiddenNodes + 1, NumberOfOutputNodes + 1) As Double
    Public WeightHiddenToOutput(NumberOfHiddenNodes + 1, NumberOfOutputNodes + 1) As Double
    Public Output(NumberOfTrainingRecords + 1, NumberOfOutputNodes + 1) As Double
    Public TrainingValues(NumberOfTrainingRecords, NumberOfTrainingFields) As Double
    Public TargetValues(NumberOfTrainingRecords, NumberOfTrainingFields) As Double

    Private Sub Button1_Click(sender As Object, e As EventArgs)
        TargetValues = LoadDataArray(TargetValues)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs)
        Dim i, j As Int16
        Dim TheData As String
        TheData = ""

        For i = 0 To TargetValues.GetUpperBound(0) - 1
            For j = 0 To TargetValues.GetUpperBound(1) - 1
                TheData = TheData + CStr(TargetValues(i, j))
            Next
            MsgBox(i & " " & "Data is >" & " " & TheData)
            TheData = ""
        Next
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

    Private Sub InitialiseWeightInputToHidden()
        Dim i, j As Int16
        For j = 1 To NumberOfHiddenNodes
            For i = 0 To NumberOfInputNodes
                DeltaWeightInputToHidden(i, j) = 0.0
                WeightInputToHidden(i, j) = Abs(2.0 * (GetRandomNumber() - 0.5) * smallwt)
            Next
        Next
    End Sub

    Private Function GetRandomNumber() As Double
        Dim NewRandom As New Random
        Dim Number As Double = Rnd()
        Return Abs(Number)
    End Function

    Private Sub InitialiseWeightHiddenToOutput()
        Dim j, k As Int16
        For k = 1 To NumberOfOutputNodes
            For j = 0 To NumberOfHiddenNodes
                DeltaWeightHiddenToOutput(j, k) = 0.0
                WeightHiddenToOutput(j, k) = Abs(2.0 * (GetRandomNumber() - 0.5) * smallwt)
            Next
        Next
    End Sub

    Private Sub BhamANN_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitialiseWeightInputToHidden()
        InitialiseWeightHiddenToOutput()
    End Sub

    Private Function sigmoid(ByVal x As Double) As Double
        Dim exp_value, return_value As Double
        exp_value = Exp(-x)
        return_value = 1 / (1 + exp_value)
        Return return_value
    End Function

    Private Sub DriveTheNeuralNetwork()
        Dim i, j, k, p, np, op, ranpat(NumberOfTrainingRecords + 1) As Int64
        Dim SumHidden(NumberOfTrainingRecords + 1, NumberOfHiddenNodes + 1), Hidden(NumberOfTrainingRecords + 1, NumberOfHiddenNodes + 1), SumO(NumberOfTrainingRecords + 1, NumberOfOutputNodes + 1) As Double
        Dim DeltaO(NumberOfOutputNodes + 1), SumDOW(NumberOfHiddenNodes + 1), DeltaH(NumberOfHiddenNodes + 1), Errored, epoch As Double
        Dim eta As Double = 0.9
        Dim alpha As Double = 0.5
        Dim x, y As Int32
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

        x = 1
        y = 0

        For epoch = 0 To MAX_EPOCH
            For p = 1 To NumberOfTrainingRecords ' GetRandomNumbermize order of training patterns
                ranpat(p) = p
            Next
            For p = 1 To NumberOfTrainingRecords
                np = p + GetRandomNumber() * (NumberOfTrainingRecords + 1 - p)
                op = ranpat(p)
                ranpat(p) = ranpat(np)
                ranpat(np) = op
            Next
            Errored = 0.0
            For np = 1 To NumberOfTrainingRecords
                p = ranpat(np)
                For j = 1 To NumberOfHiddenNodes ' compute hidden unit activations */
                    SumHidden(p, j) = WeightInputToHidden(0, j)
                    For i = 1 To NumberOfInputNodes
                        SumHidden(p, j) += TrainingValues(p, i) * WeightInputToHidden(i, j)
                    Next
                    Hidden(p, j) = sigmoid(SumHidden(p, j))
                Next
            Next
            For k = 1 To NumberOfOutputNodes   ' compute output unit activations And errors */
                SumO(p, k) = WeightHiddenToOutput(0, k)
                For j = 1 To NumberOfHiddenNodes
                    SumO(p, k) += Hidden(p, j) * WeightHiddenToOutput(j, k)
                Next
                Output(p, k) = sigmoid(SumO(p, k))
                Errored += 0.5 * (TargetValues(p, k) - Output(p, k)) * (TargetValues(p, k) - Output(p, k))   'SSE
                DeltaO(k) = (TargetValues(p, k) - Output(p, k)) * Output(p, k) * (1.0 - Output(p, k))   'Sigmoidal Outputs
            Next
            For j = 1 To NumberOfHiddenNodes     'back-propagate' errors to hidden layer */
                SumDOW(j) = 0.0
                For k = 1 To NumberOfOutputNodes
                    SumDOW(j) += WeightHiddenToOutput(j, k) * DeltaO(k)
                Next
                DeltaH(j) = SumDOW(j) * Hidden(p, j) * (1.0 - Hidden(p, j))
            Next
            For j = 1 To NumberOfHiddenNodes  'update weights WeightIH */
                DeltaWeightInputToHidden(0, j) = eta * DeltaH(j) + alpha * DeltaWeightInputToHidden(0, j)
                WeightInputToHidden(0, j) += DeltaWeightInputToHidden(0, j)
                For i = 1 To NumberOfInputNodes
                    Dim CheckDelta As Double
                    CheckDelta = DeltaWeightInputToHidden(i, j)
                    DeltaWeightInputToHidden(i, j) = eta * TrainingValues(p, i) * DeltaH(j) + alpha * DeltaWeightInputToHidden(i, j)
                    WeightInputToHidden(i, j) += DeltaWeightInputToHidden(i, j)
                    DataGridView5.Rows.Add(New String() {epoch, CheckDelta, "=", eta, "*", TrainingValues(p, i), "*", DeltaH(j), "+", alpha, "*", DeltaWeightInputToHidden(i, j)})
                Next
            Next

            For k = 1 To NumberOfOutputNodes 'update weights WeightHiddenToOutput */
                DeltaWeightHiddenToOutput(0, k) = eta * DeltaO(k) + alpha * DeltaWeightHiddenToOutput(0, k)
                WeightHiddenToOutput(0, k) += DeltaWeightHiddenToOutput(0, k)
                For j = 1 To NumberOfHiddenNodes
                    DeltaWeightHiddenToOutput(j, k) = eta * Hidden(p, j) * DeltaO(k) + alpha * DeltaWeightHiddenToOutput(j, k)
                    WeightHiddenToOutput(j, k) += DeltaWeightHiddenToOutput(j, k)
                Next
            Next
            If epoch Mod 250 = 0 Then
                DataGridView1.Rows.Add(New String() {epoch, Output(1, 1), Output(2, 1), Output(3, 1), Output(4, 1), Errored})
            End If

            If Errored < SUCCESS Then
                DataGridView1.Rows.Add(New String() {epoch, Output(1, 1), Output(2, 1), Output(3, 1), Output(4, 1), Errored})
                Exit For  'stop learning when 'near enough' */
            End If
        Next
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        DriveTheNeuralNetwork()
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

    Private Sub AlternateToSigmoid()
        Dim SumO(NumberOfTrainingRecords + 1, NumberOfOutputNodes + 1), Hidden(NumberOfTrainingRecords + 1, NumberOfHiddenNodes + 1), Errored, DeltaO(NumberOfOutputNodes + 1) As Double
        Dim p As Int64
        ''Ensure that the required number of rows are available
        'If DataGridView1.Rows.Count < Output.GetUpperBound(0) Then
        '    DataGridView1.Rows.Add((Output.GetUpperBound(0) + 1) - DataGridView1.Rows.Count)
        'End If
        For k = 1 To NumberOfOutputNodes   ' compute output unit activations And errors */
            SumO(p, k) = WeightHiddenToOutput(0, k)
            For j = 1 To NumberOfHiddenNodes
                SumO(p, k) += Hidden(p, j) * WeightHiddenToOutput(j, k)
            Next
            Output(p, k) = sigmoid(SumO(p, k))    '1.0/(1.0 + exp(-SumO[p][k])) ;  Sigmoidal Outputs */
            'Output[p][k] = SumO[p][k];      Linear Outputs */
            Errored += 0.5 * (TargetValues(p, k) - Output(p, k)) * (TargetValues(p, k) - Output(p, k))   ' SSE */
            'Error -= ( Target[p][k] * log( Output[p][k] ) + ( 1.0 - Target[p][k] ) * log( 1.0 - Output[p][k] ) ) ;    Cross-Entropy Error */
            DeltaO(k) = (TargetValues(p, k) - Output(p, k)) * Output(p, k) * (1.0 - Output(p, k))   ' Sigmoidal Outputs, SSE */
            'DeltaO[k] = Target[p][k] - Output[p][k];     Sigmoidal Outputs, Cross-Entropy Error */
            'DeltaO[k] = Target[p][k] - Output[p][k];     Linear Outputs, SSE */
        Next
    End Sub

End Class
