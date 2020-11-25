using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using AbakHelper.Integration.Models;
using AbakHelper.XeroIntegration.Models;
using Newtonsoft.Json;

namespace AbakHelper.XeroIntegration.Services
{
    public class ProjectRepository
    {
        private List<Project> _projects;

        public ProjectRepository()
        {
            _projects = File.Exists("Projects.json") ? JsonConvert.DeserializeObject<List<Project>>(File.ReadAllText("Projects.json")) : new List<Project>();
        }

        public Project Get(string name)
        {
            return _projects.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public void Add(Project project)
        {
            if (_projects.Any(c => string.Equals(c.Name, project.Name, StringComparison.OrdinalIgnoreCase)))
                throw new Exception("Can't have two projects with the same name");
            _projects.Add(project);
            Save(_projects);
        }

        public void Save(List<Project> projects = null)
        {
            if (projects != null)
                _projects = projects;
            File.WriteAllText("Projects.json", JsonConvert.SerializeObject(_projects));
        }

        public ReadOnlyCollection<Project> All()
        {
            return _projects.ToList().AsReadOnly();
        }
    }
}
