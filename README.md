# NetworkScanner
> Andrew Pineiro | August 8, 2023
---

## Summary
The purpose of this tool is to act as a network discovery scanner, but as a static binary that can be pulled into a low-privledge user computer. Current tests show that this can be pulled into a windows machine without tripping anti-virus.

## Usage

Default Values, these however can be overriden:
`ip_addr` = 127.0.0.1
`port` = 1-1000

`./NetworkScanner.exe ip_addr[/mask] [port|port,port|port-port]`

`mask` is currently limited to one octect, so the lowest mask accepted is `24`