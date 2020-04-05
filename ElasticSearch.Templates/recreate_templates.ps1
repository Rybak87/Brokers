param (
  [string] $ElasticUrl = "http://localhost:9200/"
)

$headers = @{ "Content-Type" = "application/json; charset=UTF-8" }

$messages_template = Get-Content ".\templates\messages_template.json"

try {
Invoke-WebRequest -Uri ${ElasticUrl}_template/messages -Method DELETE
} catch {
}

Invoke-WebRequest -Uri ${ElasticUrl}_template/messages -Method POST -Body ${messages_template} -Headers ${headers}
