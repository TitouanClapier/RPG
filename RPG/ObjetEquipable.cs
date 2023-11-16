using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
    public class ObjetEquipable
    {

        public string Nom { get; set; }

        public ObjetEquipable(string nom)
        {
            Nom = nom;
        }
        

    }

    public class Arme : ObjetEquipable
    {
        public Arme(string nom) : base(nom) { }

        public int Degat { get; set; }
    }

    public class Potion : ObjetEquipable
    {
        public Potion(string nom) : base(nom) { }
        public int Soin { get; set; }
    }

    public class Armure : ObjetEquipable
    {
        public Armure(string nom) : base(nom) { }
        public int Protection { get; set; }
    }

    public class Bouclier : ObjetEquipable
    {
        public Bouclier(string nom) : base(nom) { }
        public int Protection { get; set; }
    }
}
