using System;

public class LocationPath
{
    public string folderPath;
    public string fileName;

    public LocationPath(string pathName)
    {
        int index = pathName.LastIndexOf('/');
        this.folderPath = pathName.Substring(0,index);
        this.fileName = pathName.Substring(index+1);
    }

    public string FullPath()
    {
        return String.Concat(folderPath,"/",fileName);
    }
}