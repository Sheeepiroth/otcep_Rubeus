$EncryptionKey = [System.Text.Encoding]::UTF8.GetBytes("B3@t_5tr1nG_An@ly51s!")

function Encrypt-String {
    param(
        [Parameter(Mandatory=$true)]
        [string]$PlainText
    )

    $PlainTextBytes = [System.Text.Encoding]::UTF8.GetBytes($PlainText)
    $EncryptedBytes = New-Object byte[] $PlainTextBytes.Length

    for ($i = 0; $i -lt $PlainTextBytes.Length; $i++) {
        $EncryptedBytes[$i] = $PlainTextBytes[$i] -bxor $EncryptionKey[$i % $EncryptionKey.Length]
    }

    return [System.Convert]::ToBase64String($EncryptedBytes)
}

function Decrypt-String {
    param(
        [Parameter(Mandatory=$true)]
        [string]$EncryptedText
    )

    try {
        $EncryptedBytes = [System.Convert]::FromBase64String($EncryptedText)
        $DecryptedBytes = New-Object byte[] $EncryptedBytes.Length

        for ($i = 0; $i -lt $EncryptedBytes.Length; $i++) {
            $DecryptedBytes[$i] = $EncryptedBytes[$i] -bxor $EncryptionKey[$i % $EncryptionKey.Length]
        }

        return [System.Text.Encoding]::UTF8.GetString($DecryptedBytes)
    }
    catch {
        # If unable to decrypt (e.g., not a valid Base64 string or wrong key), return the original string
        # For stricter error handling, you might want to throw an exception or handle it differently.
        return $EncryptedText
    }
}


# --- Example Usage ---
$originalString = "/altservice"
Write-Host "Original String: '$originalString'"

$encrypted = Encrypt-String -PlainText $originalString
#$encrypted = "FU5cADRUAgRYCSY="
Write-Host "Encrypted String: '$encrypted'"

$decrypted = Decrypt-String -EncryptedText $encrypted
Write-Host "Decrypted String: '$decrypted'"