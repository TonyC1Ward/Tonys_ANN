Imports System.Math
Imports BirminghamArtificialNeuralNet.GlobalVariables
Imports BirminghamArtificialNeuralNet.BhamANN

Module ANNMainFunctions

    Public Sub InitialiseWeightHiddenToOutput()
        Dim j, k As Int16
        For k = 1 To NumberOfOutputNodes
            For j = 0 To NumberOfHiddenNodes
                DeltaWeightHiddenToOutput(j, k) = 0.0
                WeightHiddenToOutput(j, k) = Abs(2.0 * (GetRandomNumber() - 0.5) * smallwt)
            Next
        Next
    End Sub

    Public Sub InitialiseWeightInputToHidden()
        Dim i, j As Int16
        For j = 1 To NumberOfHiddenNodes
            For i = 0 To NumberOfInputNodes
                DeltaWeightInputToHidden(i, j) = 0.0
                WeightInputToHidden(i, j) = Abs(2.0 * (GetRandomNumber() - 0.5) * smallwt)
            Next
        Next
    End Sub

    Public Function GetRandomNumber() As Double
        Dim NewRandom As New Random
        Dim Number As Double = Rnd()
        Return Abs(Number)
    End Function

    Public Function Sigmoid(ByVal x As Double) As Double
        Dim exp_value, return_value As Double
        exp_value = Exp(-x)
        return_value = 1 / (1 + exp_value)
        Return return_value
    End Function

    Public Sub AlternateToSigmoid()
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
            Output(p, k) = Sigmoid(SumO(p, k))    '1.0/(1.0 + exp(-SumO[p][k])) ;  Sigmoidal Outputs */
            'Output[p][k] = SumO[p][k];      Linear Outputs */
            Errored += 0.5 * (TargetValues(p, k) - Output(p, k)) * (TargetValues(p, k) - Output(p, k))   ' SSE */
            'Error -= ( Target[p][k] * log( Output[p][k] ) + ( 1.0 - Target[p][k] ) * log( 1.0 - Output[p][k] ) ) ;    Cross-Entropy Error */
            DeltaO(k) = (TargetValues(p, k) - Output(p, k)) * Output(p, k) * (1.0 - Output(p, k))   ' Sigmoidal Outputs, SSE */
            'DeltaO[k] = Target[p][k] - Output[p][k];     Sigmoidal Outputs, Cross-Entropy Error */
            'DeltaO[k] = Target[p][k] - Output[p][k];     Linear Outputs, SSE */
        Next
    End Sub

    Public Sub DriveTheNeuralNetwork()
        Dim i, j, k, p, np, op, ranpat(NumberOfTrainingRecords + 1) As Int64
        Dim SumHidden(NumberOfTrainingRecords + 1, NumberOfHiddenNodes + 1), Hidden(NumberOfTrainingRecords + 1, NumberOfHiddenNodes + 1), SumO(NumberOfTrainingRecords + 1, NumberOfOutputNodes + 1) As Double
        Dim DeltaO(NumberOfOutputNodes + 1), SumDOW(NumberOfHiddenNodes + 1), DeltaH(NumberOfHiddenNodes + 1), Errored, epoch As Double
        Dim eta As Double = 0.9
        Dim alpha As Double = 0.5
        Dim x, y As Int32

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
                    Hidden(p, j) = ANNMainFunctions.Sigmoid(SumHidden(p, j))
                Next
            Next
            For k = 1 To NumberOfOutputNodes   ' compute output unit activations And errors */
                SumO(p, k) = WeightHiddenToOutput(0, k)
                For j = 1 To NumberOfHiddenNodes
                    SumO(p, k) += Hidden(p, j) * WeightHiddenToOutput(j, k)
                Next
                Output(p, k) = ANNMainFunctions.Sigmoid(SumO(p, k))
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
                    BhamANN.DataGridView5.Rows.Add(New String() {epoch, CheckDelta, "=", eta, "*", TrainingValues(p, i), "*", DeltaH(j), "+", alpha, "*", DeltaWeightInputToHidden(i, j)})
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
                BhamANN.DataGridView1.Rows.Add(New String() {epoch, Output(1, 1), Output(2, 1), Output(3, 1), Output(4, 1), Errored})
                BhamANN.Chart1.Series("Error").Points.AddXY(epoch, Errored)
            End If

            If Errored < SUCCESS Then
                BhamANN.DataGridView1.Rows.Add(New String() {epoch, Output(1, 1), Output(2, 1), Output(3, 1), Output(4, 1), Errored})
                Exit For  'stop learning when 'near enough' */
            End If
        Next
    End Sub





End Module
