using easyar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Nextreality
{
    public class ImageTarget : MonoBehaviour
    {
        public Texture2D imageTarget;

        private ImageTrackerFrameFilter Filter;

        private bool creating;
        private string directory;
        private Dictionary<string, ImageTargetController> imageTargetDic = new Dictionary<string, ImageTargetController>();

        private void Start()
        {
            
            Filter = FindObjectOfType<ImageTrackerFrameFilter>();

            directory = Path.Combine(Application.persistentDataPath, "ImageTargets");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            //LoadLocalTargets();

            StartCreateTarget();
        }

        private void LoadLocalTargets()
        {
            Dictionary<string, string> imagefilesDic = GetImagesWithDir(directory);
            foreach (var obj in imagefilesDic.Where(obj => !imageTargetDic.ContainsKey(obj.Key)))
            {
                CreateImageTarget(obj.Key, obj.Value);
            }
        }

        private Dictionary<string, string> GetImagesWithDir(string path)
        {
            Dictionary<string, string> imagefilesDic = new Dictionary<string, string>();
            foreach (var file in Directory.GetFiles(path))
            {
                if (Path.GetExtension(file) == ".jpg" || Path.GetExtension(file) == ".bmp" || Path.GetExtension(file) == ".png")
                    imagefilesDic.Add(Path.GetFileName(file), file);
            }
            return imagefilesDic;
        }

        private void CreateImageTarget(string targetName, string targetPath)
        {

            var controller = gameObject.AddComponent<ImageTargetController>();
            controller.SourceType = ImageTargetController.DataSource.ImageFile;
            controller.ImageFileSource.PathType = PathType.Absolute;
            controller.ImageFileSource.Path = targetPath;
            controller.ImageFileSource.Name = targetName;
            controller.Tracker = Filter;
            imageTargetDic.Add(targetName, controller);
        }

        public void StartCreateTarget()
        {
            StartCoroutine(CreateImageTargetFromTexture());
        }

        private IEnumerator CreateImageTargetFromTexture()
        {
            creating = true;

            yield return new WaitForEndOfFrame();

            if (!creating)
            {
                yield break;
            }

            byte[] data = imageTarget.EncodeToJPG(80);
            // Destroy(imageTarget);

            string photoName = imageTarget.name + DateTime.Now.Ticks + ".jpg";
            string photoPath = Path.Combine(directory, photoName);
            File.WriteAllBytes(photoPath, data);
            CreateImageTarget(photoName, photoPath);
            //GUIPopup.EnqueueMessage("Image saved to: " + directory + Environment.NewLine +
            //    "Image target generated: " + photoName, 3);
        }

        public void ClearAllTarget()
        {
            creating = false;

            foreach (var obj in imageTargetDic)
                Destroy(obj.Value.gameObject);
            imageTargetDic = new Dictionary<string, ImageTargetController>();

            Dictionary<string, string> imageFileDic = GetImagesWithDir(directory);
            foreach (var path in imageFileDic)
                File.Delete(path.Value);
        }
    }
}