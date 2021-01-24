netsh http add urlacl url=https://+:59998/ user=EVERYONE
makecert.exe -sk RootCA -sky signature -pe -n CN=localhost -r -sr LocalMachine -ss Root PE.cer
makecert.exe -sk server -sky exchange -pe -n CN=localhost -ir LocalMachine -is Root -ic PE.cer -sr LocalMachine -ss PendarEditor.cer
netsh.exe http add sslcert ipport=0.0.0.0:59998 certhash=34CD40020C8CDE33090C90CAFB8FFF0091D8E076 appid={0ee35094-d45b-4fd1-b5fb-c775ee1357fa}


