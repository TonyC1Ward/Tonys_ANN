Public Class GlobalVariables
    Public Const NumberOfInputNodes As Int16 = 2
    Public Const NumberOfHiddenNodes As Int16 = 2
    Public Const smallwt As Double = 0.5
    Public Const NumberOfOutputNodes As Int16 = 1
    Public Const NumberOfTrainingRecords As Int16 = 4
    Public Const SUCCESS As Double = 0.0002
    Public Const MAX_EPOCH As Int64 = 2000000
    Public Const NumberOfTrainingFields As Int64 = 3
    Public Shared DeltaWeightInputToHidden(NumberOfInputNodes + 1, NumberOfHiddenNodes + 1) As Double
    Public Shared WeightInputToHidden(NumberOfInputNodes + 1, NumberOfHiddenNodes + 1) As Double
    Public Shared DeltaWeightHiddenToOutput(NumberOfHiddenNodes + 1, NumberOfOutputNodes + 1) As Double
    Public Shared WeightHiddenToOutput(NumberOfHiddenNodes + 1, NumberOfOutputNodes + 1) As Double
    Public Shared Output(NumberOfTrainingRecords + 1, NumberOfOutputNodes + 1) As Double
    Public Shared TrainingValues(NumberOfTrainingRecords, NumberOfTrainingFields) As Double
    Public Shared TargetValues(NumberOfTrainingRecords, NumberOfTrainingFields) As Double
    Public Shared Hidden(NumberOfTrainingRecords + 1, NumberOfHiddenNodes + 1)
End Class
