using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace HouseDesign.Classes
{
    public class Category
    {
        public String Name { get; set; }
        public List<Category> SubCategories { get; set; }
        public List<String> Files { get; set; }
        public String Path { get; set; }

        public Category(String name, String path)
        {
            this.Name = name;
            if(path.Length==0)
            {
                this.Path = @"D:\Licenta\HouseDesign\HouseDesign\Exports\" + Name;
            }
            else
            {
                this.Path = path;
            }           
            SubCategories = new List<Category>();
            Files = new List<String>();
            InitializeCategory(this);
        }

        public void InitializeCategory(Category mainCategory)
        {            
            try
            {
                foreach (String file in Directory.GetFiles(mainCategory.Path))
                {                    
                    mainCategory.Files.Add(file);
                }
                foreach (String directory in Directory.GetDirectories(mainCategory.Path))
                {
                    String[] tokens = directory.Split('.');
                    String actualDrectoryName = tokens[0].Split('\\').Last();
                    Category category = new Category(actualDrectoryName, directory);
                    mainCategory.SubCategories.Add(category);
                }
            }
            catch (System.Exception excpt)
            {
                MessageBox.Show(excpt.Message);
            }
        }
    }
}
