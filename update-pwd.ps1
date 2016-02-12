$user = "neo4j"
$pass= "neo4j"
$uri = "http://localhost:7474/user/neo4j/password"
$json = "{
            ""password"" : ""password""
          }" 


$base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $user,$pass)))

Invoke-RestMethod -Uri $uri -Method Post -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo)} -ContentType "application/json; charset=UTF-8" -Body $json 

