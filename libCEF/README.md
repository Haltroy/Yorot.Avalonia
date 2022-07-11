# LibCEF

This folder contains a Python script that will download the
Chromium Embedded Framework binaries required by
CefNet into here.

It will download all available binary files for both Windows and Linux
into this folder, extract them and reorganize them.

However, Yorot doesn't looks into this directory for those files.
Yorot checks this folder instead:

- `[Yorot Data folder]\cef` 
  - (Windows: `C:\Users\[your username]\AppData\Roaming\.yorot\cef\`)
  - (Linux: `/home/[Your User Name]/.config/.yorot/cef`)

In the future, this will change and you no longer need to move those files
into the data folder. Instead, you will just run the Python script to get
all of them and when you build them, they will be inside the bin folder instead.
