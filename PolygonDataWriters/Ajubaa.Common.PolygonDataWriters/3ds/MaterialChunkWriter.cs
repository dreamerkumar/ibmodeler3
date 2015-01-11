using System;
using System.IO;
using System.Windows.Media;

namespace Ajubaa.Common.PolygonDataWriters._3ds
{
    internal static class MaterialChunkWriter
    {
        internal const int ColorChunkLength = 9;
        internal const int MatShinePercentChunkLength = 14;
        internal const int PercentChunkLength = 8;
        internal const int SpecificColorChunkLength = 24;
        internal const int MaterialTypeChunkLength = 8;
        internal const int TransparencyChunkLength = 14;
        internal const int TextureChnkLenWithoutFileNameLen = 68;

        internal const string MaterialName = "mat1";
        internal const ushort MaterialNameLen = 5;

        internal static int GetMaterialListChunkLength(ushort ushNoOfFaces)
        {
            return (6 + MaterialNameLen + 2 + 2 * ushNoOfFaces);
            // c_cushMaterialNameLen for the name mat1, 2 for the entry for number of indices and then 2 bytes per entry
        }

        internal static void WriteMaterialChunk(Color rgbModelColor, bool addTexture, string textureFileName, BinaryWriter file)
        {
            //for sanity check
            var checkLengthInitial = file.BaseStream.Length;

            //Go to the end
            file.Seek(0, SeekOrigin.End);

            const ushort idMaterial = 0xAFFF; // material chunk under 0x3D3D

            var textureChunkLength = 0;
            if (addTexture)
            {
                var intTextureFileNameLength = (textureFileName.Length + 1);
                //Length is increased by one to allow writing the null character at the end which is required
                textureChunkLength = TextureChnkLenWithoutFileNameLen +
                                        intTextureFileNameLength;
            }

            //Calculate the required chunk lengths
            const int materialNameChunkLen = 6 + MaterialNameLen;

            var materialChunkLen = 6 
                + materialNameChunkLen
                + 3 * SpecificColorChunkLength
                + 2 * MatShinePercentChunkLength
                + TransparencyChunkLength
                + MaterialTypeChunkLength
                + textureChunkLength;

            //Write the material chunk
            file.Write(idMaterial);
            file.Write(materialChunkLen);

            //Compulsory sub chunks
            //
            //		Id    type     function
            //
            //		A000   asciiz   material name   [the name terminated by a null char]
            //		A010   S        ambient color   [RGB1 and RGB2]
            //		A020   S        diffuse color       idem
            //		A030   S        specular color      idem
            //		A040   S        shininess       [amount of]
            //		A041   S        shin. strength     "
            //		A050   S        transparency       "
            //		A052   S        trans. falloff     "
            //		A053   S        reflect blur       "
            //		A100   intsh    material type   [1=flat 2=gour. 3=phong 4=metal]
            //

            //Write the material name chunk
            WriteMaterialNameChunk(materialNameChunkLen, file);

            //Write the color chunks
            WriteAmbientColor(rgbModelColor.R, rgbModelColor.G, rgbModelColor.B, file);
            WriteDiffuseColor(rgbModelColor.R, rgbModelColor.G, rgbModelColor.B, file);
            WriteSpecularColor(rgbModelColor.R, rgbModelColor.G, rgbModelColor.B, file);

            WriteMatShinePercentChunks(50, 50, file);

            WriteTransparencyInformation(0, file); //parameter: percentage

            WriteMaterialTypeChunk(4, file); //material type [1=flat 2=gour. 3=phong 4=metal]

            if (addTexture)
                WriteTextureChunk(textureFileName, file);

            //sanity check
            var checkLengthLater = file.BaseStream.Length;
            if (checkLengthLater - checkLengthInitial != materialChunkLen)
                throw new Exception("Invalid length of material chunk");
        }

        private static void WriteMaterialNameChunk(int materialNameChunkLen, BinaryWriter file)
        {
            //for sanity check
            var checkLengthInitial = file.BaseStream.Length;

            const ushort idMaterialName = 0xA000;
            file.Write(idMaterialName);
            file.Write(materialNameChunkLen);
            file.Write(MaterialName.ToCharArray()); //Writer(string) writes a length-prefixed string
            file.Write('\0');

            //sanity check
            var checkLengthLater = file.BaseStream.Length;
            if (checkLengthLater - checkLengthInitial != materialNameChunkLen)
                throw new Exception("Invalid length of material name chunk");
        }

        internal static void WriteMatInformationForObjFaces(ushort noOfFaces, int mapListChunkLength, BinaryWriter file)
        {
            file.Seek(0, SeekOrigin.End); //We have to append our data to the end of the file

            //Write the face material list chunk
            const ushort idFacesMaterialList = 0x4130; //contains the information on the material used
            file.Write(idFacesMaterialList);
            file.Write(mapListChunkLength);
            file.Write(MaterialName.ToCharArray()); //Writer(string) writes a length-prefixed string
            file.Write('\0');
            file.Write(noOfFaces);
            for (ushort ctr = 0; ctr < noOfFaces; ctr++)
                file.Write(ctr);
        }

        private static void WriteColorChunk(byte btR, byte btG, byte btB, BinaryWriter file)
        {
            const ushort idColor24 = 0x11;
            file.Write(idColor24);
            file.Write(ColorChunkLength);
            file.Write(btR);
            file.Write(btG);
            file.Write(btB);
        }

        private static void WriteGammaColorChunk(byte r, byte g, byte b, BinaryWriter file)
        {
            const ushort idGammaColor24 = 0x12;
            file.Write(idGammaColor24);
            file.Write(ColorChunkLength);
            file.Write(r);
            file.Write(g);
            file.Write(b);
        }

        private static void WriteAmbientColor(byte r, byte g, byte b, BinaryWriter file)
        {
            const ushort idAmbient = 0xA010;
            WriteSpecificColor(idAmbient, r, g, b, file);
        }

        private static void WriteSpecularColor(byte r, byte g, byte b, BinaryWriter file)
        {
            const ushort idSpecular = 0xA030;
            WriteSpecificColor(idSpecular, r, g, b, file);
        }

        private static void WriteDiffuseColor(byte r, byte g, byte b, BinaryWriter file)
        {
            const ushort idDiffuse = 0xA020;
            WriteSpecificColor(idDiffuse, r, g, b, file);
        }

        private static void WriteSpecificColor(ushort id, byte r, byte g, byte b, BinaryWriter file)
        {
            file.Write(id);
            file.Write(SpecificColorChunkLength);
            WriteColorChunk(r, g, b, file);
            WriteGammaColorChunk(r, g, b, file);
        }

        private static void WriteMatShinePercentChunks(ushort percent1, ushort percent2, BinaryWriter file)
        {
            //Write shininess percent chunks
            const ushort idShininess = 0xA040;
            file.Write(idShininess);
            file.Write(MatShinePercentChunkLength);
            WritePercentageChunk(percent1, file);

            const ushort idShin2Pct = 0xA041;
            file.Write(idShin2Pct);
            file.Write(MatShinePercentChunkLength);
            WritePercentageChunk(percent2, file);
        }

        private static void WritePercentageChunk(ushort ushPercentage, BinaryWriter file)
        {
            const ushort idIntPercentage = 0x30;
            file.Write(idIntPercentage);
            file.Write(PercentChunkLength);
            file.Write(ushPercentage);
        }

        private static void WriteTransparencyInformation(ushort ushTransparencyPercent, BinaryWriter file)
        {
            const ushort idTransparency = 0xA050;
            file.Write(idTransparency);
            file.Write(TransparencyChunkLength);
            WritePercentageChunk(ushTransparencyPercent, file);
        }

        private static void WriteMaterialTypeChunk(ushort ushMaterialType, BinaryWriter file)
        {
            const ushort idMaterialType = 0xA100;
            file.Write(idMaterialType);
            file.Write(MaterialTypeChunkLength);
            file.Write(ushMaterialType);
        }

        private static void WriteTextureChunk(string textureFileName, BinaryWriter file)
        {
            var textureFileNameLen = (textureFileName.Length + 1); //+ 1 for the null character

            const ushort idTexture1 = 0xA200; // texture1 chunk

            //Write the texture chunk
            file.Write(idTexture1);
            var textureChunkLength = TextureChnkLenWithoutFileNameLen +
                                        textureFileNameLen;
            file.Write(textureChunkLength);

            WritePercentageChunk(100, file);

            //Write the texture filename chunk
            const ushort idTexfilename = 0xA300; // NULL terminated filename of texture1 (****probably requires a dos name)
            file.Write(idTexfilename);
            var texFileNameChunkLen = textureFileNameLen + 6;
            file.Write(texFileNameChunkLen);
            //Write the null terminated filename (GetBuffer returns the specified count of characters and also appends a \0 at the end of it so the returned buffer size is one more than the passed count
            file.Write(textureFileName.ToCharArray()); //Writer(string) writes a length-prefixed string
            file.Write('\0');
            // nMinBufLength specifies the minimum size of the character buffer in characters. This value does not include space for a null terminator (ref: http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dv_wcemfc4/html/aflrfcstringcolcolgetbuffer.asp)

            const ushort idMatMapTiling = 0xA351; //size=8
            file.Write(idMatMapTiling);
            const int matMapTilingChunkLen = 8;
            file.Write(matMapTilingChunkLen);
            const ushort tilingFlags = 0;
            file.Write(tilingFlags);

            const int lengthForUvChunks = 10;
            const float uvScale = 1.0f;
            const float uvOffset = 0.0f;

            const ushort idTexMapUscale = 0xA354;
            file.Write(idTexMapUscale);
            file.Write(lengthForUvChunks);
            file.Write(uvScale);

            const ushort idTexMapUoffset = 0xA358;
            file.Write(idTexMapUoffset);
            file.Write(lengthForUvChunks);
            file.Write(uvOffset);

            const ushort idTexMapVscale = 0xA356;
            file.Write(idTexMapVscale);
            file.Write(lengthForUvChunks);
            file.Write(uvScale);

            const ushort idTexMapVoffset = 0xA35A;
            file.Write(idTexMapVoffset);
            file.Write(lengthForUvChunks);
            file.Write(uvOffset);
        }
    }
}
