import os, sys, wget, tarfile, shutil

cef_version = "103.0.8+g444ebe7+chromium-103.0.5060.66"

# win-x64 -> windows64
# win-x86 -> windows32
# win-arm64 -> windowsarm64
# linux-x64 -> linux64
# linux-arm64 -> linuxarm64
# linux-arm -> linuxarm

def download(url, file_name):
    wget.download(url, file_name)
    

def getArch(cefver,arch,loc):
    print("-----")
    print("Start: "+ arch) 
    url = "https://cef-builds.spotifycdn.com/cef_binary_" + cefver + "_" + arch + "_client.tar.bz2"
    fileloc =  os.path.join(loc,"cef_binary_" + cefver + "_" + arch + ".tar.bz2")
    folderloc =  os.path.join(loc,"cef_" + arch)
    remloc =  os.path.join(folderloc,"cef_binary_" + cefver + "_" + arch + "_client")
    moveloc =  os.path.join(remloc,"Release")
   
    
    
    print("Download: " + url.replace("+","%2B"))
    download(url, fileloc)


    ceftar = tarfile.open(fileloc)

    if not os.path.exists(folderloc):
        print("Directory Create: " + folderloc)
        os.makedirs(folderloc)

    print("Extract: " + fileloc)
    ceftar.extractall(folderloc)
    ceftar.close()

    os.remove(fileloc)
    
    print("Reorganize from: "+ moveloc)
    
    allfiles = os.listdir(moveloc)
  
    for f in allfiles:
        os.rename(os.path.join(moveloc, f), os.path.join(folderloc, f))

    
    shutil.rmtree(remloc, ignore_errors=True)
    
    print("Done: "+ arch)
    print("-----")

def downloadAll(cefver, loc):
    archs = ["windows64", "windows32", "windowsarm64", "linux64", "linuxarm64", "linuxarm"]
    for arch in archs:
        getArch(cefver, arch, loc)

def downloadAllHere(cefver):
    loc = os.path.dirname(os.path.realpath(sys.argv[0]))
    downloadAll(cefver, loc)

if __name__ == '__main__':
   downloadAllHere(cef_version)