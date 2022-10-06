import os, sys, wget, tarfile, shutil

# win-x64 -> windows64
# win-x86 -> windows32
# win-arm64 -> windowsarm64
# linux-x64 -> linux64
# linux-arm64 -> linuxarm64
# linux-arm -> linuxarm

def download(url, file_name):
    wget.download(url, file_name)
    

def getArch(cefver,arch,loc, beta):
    print("-----")
    print("Start: "+ arch) 
    url = "https://cef-builds.spotifycdn.com/cef_binary_" + cefver + "_" + arch + "_client.tar.bz2"
    if beta:
        url = "https://cef-builds.spotifycdn.com/cef_binary_" + cefver + "_" + arch + "_beta_client.tar.bz2"
    fileloc =  os.path.join(loc,"cef_binary_" + cefver + "_" + arch + ".tar.bz2")
    folderloc =  os.path.join(loc,"cef_" + arch)
    remloc =  os.path.join(folderloc,"cef_binary_" + cefver + "_" + arch + "_client")
    if beta:
        remloc =  os.path.join(folderloc,"cef_binary_" + cefver + "_" + arch + "_beta_client")
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

def downloadAll(cefver, loc, beta):
    archs = ["windows64", "windows32", "windowsarm64", "linux64", "linuxarm64", "linuxarm"]
    for arch in archs:
        getArch(cefver, arch, loc, beta)

def downloadAllHere(cefver, beta):
    loc = os.path.dirname(os.path.realpath(sys.argv[0]))
    downloadAll(cefver, loc, beta)

if __name__ == '__main__':
   if len(sys.argv) < 1:
        print("USAGE: populate.py [CEF Version] [--beta]")
        print("EXAMPLE: populate.py 105.3.18+gbfea4e1+chromium-105.0.5195.52")
        exit()
   downloadBeta = False
   if len(sys.argv) > 1:
        if sys.argv[2] == "--beta":
            downloadBeta = True
   downloadAllHere(sys.argv[1],downloadBeta)