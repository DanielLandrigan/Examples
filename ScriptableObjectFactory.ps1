# This script needs to be ran in PowerShell within the Unity project
# Define the path to the CSV file
$csvFilePath = "C:\Users\danie\Crafting\Assets\Scripts\ItemSOs\ItemsSheet.csv"

# Define the path where you want to save the Scriptable Objects
$outputPath = "C:\Users\danie\Crafting\Assets\Scripts\ItemSOs"

# Load Unity Editor assemblies
$unityEditorAssembly = (Get-Process -name Unity).Modules | Where-Object { $_.ModuleName -eq 'UnityEditor.dll' }
Add-Type -Path $unityEditorAssembly.FileName

# Create the output directory if it doesn't exist
New-Item -ItemType Directory -Force -Path $outputPath

# Read the CSV file
$data = Import-Csv $csvFilePath

# Iterate through the rows in the CSV file
foreach ($row in $data) {
    # Define the scriptable object file name
    $scriptableObjectFileName = "$outputPath/$($row.ItemName)_SO.asset"

    # Create a new Scriptable Object
    $scriptableObject = New-Object -TypeName "UnityEngine.ScriptableObject"

    # Add properties to the Scriptable Object
    $scriptableObject | Add-Member -MemberType NoteProperty -Name "ItemName" -Value $row.ItemName
    $scriptableObject | Add-Member -MemberType NoteProperty -Name "Stackable" -Value [System.Convert]::ToBoolean($row.Stackable)
    $scriptableObject | Add-Member -MemberType NoteProperty -Name "StackSize" -Value [System.Convert]::ToInt32($row.StackSize)
    $scriptableObject | Add-Member -MemberType NoteProperty -Name "NoDrop" -Value [System.Convert]::ToBoolean($row.NoDrop)
    $scriptableObject | Add-Member -MemberType NoteProperty -Name "BuyValue" -Value [System.Convert]::ToInt32($row.BuyValue)
    $scriptableObject | Add-Member -MemberType NoteProperty -Name "SellValue" -Value [System.Convert]::ToInt32($row.SellValue)

    # Save the Scriptable Object to a file
    [UnityEditor.AssetDatabase]::CreateAsset($scriptableObject, $scriptableObjectFileName)
}

# Refresh the Unity editor to make sure it recognizes the new assets
[UnityEditor.AssetDatabase]::Refresh()
