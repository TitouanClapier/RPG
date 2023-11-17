using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
    public abstract class Personnage
    {

            public string Nom { get; set; }
            public int Vie { get; set; }
            public int VieMax { get; set; }
            public int Degat { get; set; }


            public Personnage(string nom, int vie, int degat)
            {
                Nom = nom;
                Vie = vie;
                VieMax = vie;
                Degat = degat;
            }

            public abstract void Attaquer(Personnage cible);
            public abstract void Defendre();
        
    }

    public class Ennemi : Personnage
    {
        private bool peutAttaquer = true;
        public Rarete Rarete { get; set; }
        public double TauxDrop { get; set; }
        public bool ProchainTourAttaqueChargee { get; set; }
        public bool AttaqueChargeeEnCours { get; set; }
        private bool attaqueChargeeProchainTour = false;

        public Ennemi(string nom, int vie, int degat, Rarete rarete, double tauxDrop) : base(nom, vie, degat)
        {
            Rarete = rarete;
            TauxDrop = tauxDrop;
        }

        public bool AttaqueChargeeProchainTour
        {
            get { return attaqueChargeeProchainTour; }
            set { attaqueChargeeProchainTour = value; }
        }

        public override void Attaquer(Personnage cible)
        {
            int attaqueFinale = 0; 
            int degatsBruts = Degat;
            Joueur joueurCible = cible as Joueur;

            if (AttaqueChargeeEnCours)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{Nom} a utilisé son attaque chargée ! ");
                Console.ResetColor();
                degatsBruts = Degat / 3;
                attaqueFinale = Degat - degatsBruts;
                
                Console.WriteLine();  // Ajout du saut de ligne
                AttaqueChargeeEnCours = false;
            }

            if (joueurCible != null)
            {
                if (joueurCible.ActionSoin)
                {
                    joueurCible.UtiliserPotion();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"! {Nom} récupère de la vie avec une potion de soin.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{Nom}");
                    Console.ResetColor();
                    Console.Write(" va attaquer ");
                    Console.WriteLine();

                    if (joueurCible.EnDefense)
                    {
                        attaqueFinale += degatsBruts - joueurCible.Defense;
                        Console.Write("avec son bouclier défend ");

                        joueurCible.EnDefense = false;
                    }
                    else
                    {
                        if (AttaqueChargeeEnCours)
                        {
                            attaqueFinale += degatsBruts - joueurCible.Defense;
                            degatsBruts = Degat / 3;
                        }
                        else
                        {
                            attaqueFinale += degatsBruts - joueurCible.Defense;
                        }
                    }
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{Nom} ");
                Console.Write("attaque ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{cible.Nom} ");

                if (AttaqueChargeeEnCours)
                {
                    attaqueFinale += degatsBruts;
                    degatsBruts = Degat / 3;
                }
                else
                {
                    attaqueFinale += degatsBruts;
                }
            }

            bool attaqueForte = !AttaqueChargeeEnCours && new Random().Next(1, 4) == 1;

            if (attaqueForte)
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{Nom} ");
                Console.ResetColor();
                Console.Write("prépare une attaque puissante ! ");
                Console.WriteLine();
                Degat = degatsBruts;
                attaqueFinale = 0;
                Degat *= 3;
                degatsBruts = Degat / 3;
                AttaqueChargeeEnCours = true;
            }
            else
            {
                Degat = degatsBruts;
            }

            
            
            


            attaqueFinale = Math.Max(attaqueFinale, 0);

            cible.Vie -= attaqueFinale;

            Console.ResetColor();


            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{Nom} ");
            Console.ResetColor();
            Console.Write($"a infligé {attaqueFinale} points de dégâts à ");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{cible.Nom} ");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{cible.Nom} ");
            Console.ResetColor();
            Console.Write($"a maintenant {cible.Vie} points de vie.");
            Console.ResetColor();
        }

        public override void Defendre()
        {
        }

        public bool Attaquer()
        {
            return peutAttaquer;
        }

        public double CalculTauxDrop()
        {
            switch (Rarete)
            {
                case Rarete.Normal:
                    return TauxDrop;
                case Rarete.Rare:
                    return TauxDrop * 1.5;
                case Rarete.Epic:
                    return TauxDrop * 2.0;
                case Rarete.Legendaire:
                    return TauxDrop * 4.0;
                case Rarete.Mythique:
                    return TauxDrop * 10.0;
                default:
                    return TauxDrop;
            }
        }
    }

    public class Joueur : Personnage
    {
        public List<ObjetEquipable> Inventaire { get; set; }
        public int Experience { get; set; }
        public int Niveau { get; set; }
        public int ExperienceNecessaire { get; private set; }

        public bool AttaqueFortePT { get; set; }

        public void AttaqueForte()
        {
            AttaqueFortePT = true;
        }

        public int Defense
        {
            get
            {
                int defenseTotale = 0;
                foreach (var objet in Inventaire)
                {
                    if (objet is Armure armure)
                    {
                        defenseTotale += armure.Protection;
                    }
                    else if (objet is Bouclier bouclier)
                    {
                        defenseTotale += bouclier.Protection;
                    }
                }
                return defenseTotale;
            }
        }



        public bool EnDefense { get; set; }
        public bool ActionSoin { get; set; }

        public Joueur(string nom, int vie, int degat) : base(nom, vie, degat)
        {
            Inventaire = new List<ObjetEquipable>();
            Experience = 0;
            Niveau = 1;
            ExperienceNecessaire = ExperienceRequise();
        }

        public override void Attaquer(Personnage cible)
        {
            string NomArmeEquipee = ArmeEquipee().Nom;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{Nom} ");
            Console.ResetColor();

            Console.Write($"attaque ");

            Console.ForegroundColor = ConsoleColor.Red; Console.Write($"{cible.Nom}");
            Console.ResetColor();

            if (string.IsNullOrEmpty(NomArmeEquipee))
            {
                Console.WriteLine(" avec ses poings !");
                cible.Vie -= Degat;
            }
            else
            {
                Console.WriteLine($" avec {NomArmeEquipee} !");
                cible.Vie -= Degat + ArmeEquipee().Degat;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{cible.Nom} ");
            Console.ResetColor();
            Console.WriteLine($"a maintenant {cible.Vie} points de vie.");
        }

        public override void Defendre()
        {
            Bouclier bouclierEquipe = Inventaire.OfType<Bouclier>().FirstOrDefault();

            if (bouclierEquipe != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{Nom} ");
                Console.ResetColor();

                Console.WriteLine($"se défend avec son bouclier {bouclierEquipe.Nom} !");
                EnDefense = true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{Nom} ");
                Console.ResetColor();

                Console.WriteLine($"n'a pas de bouclier équipé !");
            }
        }

        public void UtiliserPotion()
        {
            var potion = Inventaire.Find(o => o is Potion) as Potion;
            if (potion != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{Nom} ");
                Console.ResetColor();

                Console.WriteLine($"utilise une potion de {potion.Nom} !");
                Vie += potion.Soin;
                if (Vie > VieMax)
                {
                    Vie = VieMax;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{Nom} ");
                Console.ResetColor();

                Console.WriteLine($"a maintenant {Vie} points de vie.");

                ActionSoin = true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{Nom} ");
                Console.ResetColor();

                Console.WriteLine($"ne peut pas utiliser de potion, aucune potion dans l'inventaire.");
            }
        }

        /*
        public bool Courir()
        {
            Random random = new Random();
            return random.Next(2) == 0;
        }
        */

        public void AjouterObjet(ObjetEquipable objet)
        {
            Inventaire.Add(objet);
            Console.WriteLine($"{Nom} a obtenu {objet.Nom} !");
            EquiperMeilleurObjet();
        }

        private void EquiperMeilleurObjet()
        {
            var meilleureArme = Inventaire.OfType<Arme>().OrderByDescending(a => a.Degat).FirstOrDefault();
            var meilleureArmure = Inventaire.OfType<Armure>().OrderByDescending(a => a.Protection).FirstOrDefault();
            var meilleurBouclier = Inventaire.OfType<Bouclier>().OrderByDescending(b => b.Protection).FirstOrDefault();
            var meilleurePotion = Inventaire.OfType<Potion>().OrderByDescending(p => p.Soin).FirstOrDefault();

            Inventaire.Clear();

            if (meilleureArme != null)
                Inventaire.Add(meilleureArme);
            if (meilleureArmure != null)
                Inventaire.Add(meilleureArmure);
            if (meilleurBouclier != null)
                Inventaire.Add(meilleurBouclier);
            if (meilleurePotion != null)
                Inventaire.Add(meilleurePotion);
        }

        public Arme ArmeEquipee()
        {
            return Inventaire.FirstOrDefault(o => o is Arme) as Arme ?? new Arme("Poing");
        }


        private int ExperienceRequise()
        {
            return 50 * Niveau;
        }

        public void GagnerExperience(Ennemi ennemi)
        {
            int xpGagnee = 0;

            switch (ennemi.Rarete)
            {
                case Rarete.Normal:
                    xpGagnee = 10;
                    break;
                case Rarete.Rare:
                    xpGagnee = 30;
                    break;
                case Rarete.Epic:
                    xpGagnee = 50;
                    break;
                case Rarete.Legendaire:
                    xpGagnee = 300;
                    break;
                default:
                    xpGagnee = 0;
                    break;
            }

            Experience += xpGagnee;
            Console.WriteLine($"{Nom} a gagné {xpGagnee} XP !");

            while (Experience >= ExperienceNecessaire)
            {
                Experience -= ExperienceNecessaire;
                Niveau++;
                ExperienceNecessaire = ExperienceRequise();
                StatsNiveaux();
            }
        }

        private void StatsNiveaux()
        {
            VieMax += 2;
            Vie = VieMax;
            Degat += 1;

            Console.WriteLine($"{Nom} est passé au niveau {Niveau} !");
        }
    }
}
