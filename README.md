# NetworkScanner
> Andrew Pineiro | August 8, 2023
---

## Summary
The purpose of this tool is to act as a network discovery scanner, but as a static binary that can be pulled into a low-privledge user computer. Current tests show that this can be pulled into a windows machine without tripping anti-virus.

## Usage

`./NetworkScanner.exe ip_address [options]` 

### Options
`--help,-h` - Displays the help text \
`-p[1-65535]` - [DEFAULT: 1-1000] Supplies a port range for scanning. Accepts a range (#-#), single port (#), or comma seperated within quotes ("#,#,#") \
`-pN` - Perform a portscan without caring about ping replies. \
`/[24...32]` - Supplies a subnet mask for ip range scanning. \
`-d, --debug` - Enables debugging messages \
`-A` - Enabled aggressive port scanning mode