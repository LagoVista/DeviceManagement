Param([string]$storageKey)

function Add-Entity() {
    [CmdletBinding()]
    param(
       $table,
       [String]$partitionKey,
       [String]$rowKey,
       [String]$text,
       [String]$status
    )

    $insertedDateStamp = Get-Date

    $entity = New-Object -TypeName Microsoft.WindowsAzure.Storage.Table.DynamicTableEntity -ArgumentList $partitionKey, $rowKey
    $entity.Properties.Add("Text", $text)
    $entity.Properties.Add("Status", $status)
    $entity.Properties.Add("Notes", "")
    $entity.Properties.Add("Issues", "none")
    $entity.Properties.Add("CreationDate", $insertedDateStamp)
    $entity.Properties.Add("LastUpdatedDate", $insertedDateStamp) 
    $entity.Properties.Add("Edits", "false")
    $entity.Properties.Add("Reviewed", "false")
    $entity.Properties.Add("Approved", "false")

    $result = $table.CloudTable.Execute([Microsoft.WindowsAzure.Storage.Table.TableOperation]::Insert($entity))
}


function Update-Entity() {
    [CmdletBinding()]
    param(
       $table,
       [String]$partitionKey,
       [String]$rowKey,
       [String]$text,
       [String]$status,
       [DateTime]$creationDate
    )

    $updatedDateStamp = Get-Date

    $entity = New-Object -TypeName Microsoft.WindowsAzure.Storage.Table.DynamicTableEntity -ArgumentList $partitionKey, $rowKey
    $entity.Properties.Add("Text", $text)
    $entity.Properties.Add("Status", $status)
    $entity.Properties.Add("Notes", "")
    $entity.Properties.Add("Issues", "none")
    $entity.Properties.Add("CreationDate", $creationDate)
    $entity.Properties.Add("LastUpdatedDate", $updatedDateStamp)
    $entity.Properties.Add("Edits", "false")
    $entity.Properties.Add("Reviewed", "false")
    $entity.Properties.Add("Approved", "false")
 
    $result = $table.CloudTable.Execute([Microsoft.WindowsAzure.Storage.Table.TableOperation]::InsertOrReplace($entity))
}

function Process-ResourceEntry() {
    [CmdletBinding()]
    param(
       $table,
       [String]$partitionKey,
       [String]$rowKey,
       [String]$text
    )

    $list = New-Object System.Collections.Generic.List[string] 
    $list.Add("Text") 
    $list.Add("Status") 
    $list.Add("CreationDate") 
    $list.Add("LastUpdated") 

    $results = $table.CloudTable.Execute([Microsoft.WindowsAzure.Storage.Table.TableOperation]::Retrieve($partitionKey, $rowKey, $list))
    #Write-Output $results.Result.Properties["CreationDate"].DateTime

    if($results.HttpStatusCode -eq 404){
        Add-Entity -Table $table -PartitionKey $partitionKey -RowKey $rowKey -Text $text -Status new
        Write-Output "Add new entity: " $rowKey
    }
    else {
        $inserted  = $results.Result.Properties["CreationDate"].DateTime;
        $existingText = $results.result.Properties["Text"].StringValue;
        
        if($existingText -eq $text) {
            $currentStatus = $results.result.Properties["Status"].StringValue;
            $updated  = $results.Result.Properties["LastUpdatedDate"].DateTime;
        }
        else {
            Write-Output "Entity Updated: " $rowKey
            Update-Entity -Table $table -PartitionKey $partitionKey -RowKey $rowKey -Text $item.value -Status updated -CreationDate $inserted
        }
   }
 }

$StorageAccountName = "nuviotresources"
#$StorageAccountKey = Get-AzureStorageKey -StorageAccountName $StorageAccountName
$Ctx = New-AzureStorageContext $StorageAccountName -StorageAccountKey $storageKey
$TableName = "allresources"

Write-Output $ctx

$scriptPath = Split-Path $MyInvocation.MyCommand.Path
Set-Location $scriptPath

 $SubscriptionName = "Primary"

 # Give a name to your new storage account. It must be lowercase!
 $StorageAccountName = "nuviotresources"

 $table = Get-AzureStorageTable –Name $TableName -Context $Ctx -ErrorAction Ignore

#Create a new table if it does not exist.
if ($table -eq $null)
{
   $table = New-AzureStorageTable –Name $TableName -Context $Ctx
}


$children = gci ./ -recurse *.resx 
foreach( $child in $children){

    $nuspecFile = gi $child.fullName
    [xml] $content = Get-Content $nuspecFile
     foreach($item in $content.root.data){
        Process-ResourceEntry -Table $table -PartitionKey $child.name -RowKey $item.name -Text $item.value
     }
}