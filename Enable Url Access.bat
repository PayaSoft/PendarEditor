@ECHO Off

ECHO Deleting
netsh http delete urlacl url=http://+:59999/
if NOT "%1" EQU "/u" (
  ECHO Setting
  netsh http add urlacl url=http://+:59999/ user=EveryOne listen=yes
  IF ERRORLEVEL == 0 ECHO DONE
  IF NOT ERRORLEVEL == 0 (
	ECHO Error
	pause
  )
)
ECHO Deleting
netsh http delete urlacl url=http://+:59991/
if NOT "%1" EQU "/u" (
  ECHO Setting
  netsh http add urlacl url=http://+:59991/ user=EveryOne listen=yes
  IF ERRORLEVEL == 0 ECHO DONE
  IF NOT ERRORLEVEL == 0 (
	ECHO Error
	pause
  )
)

