using System;
using System.Collections.Generic;
using System.Linq;

namespace RPG
{
    class Programme
    {
        static int NombreMort = 0;
        static Joueur JoueurActuel = new Joueur("Patouan", 100, 2);
        static List<Ennemi> Ennemis = GenererEnnemis();

        static void Main(string[] args)
        {
            Commencer();
        }

        static void Commencer()
        {

            Introduction(); RencontrerEnnemi();
            Ennemi ennemiRencontre = Ennemis[0];

            while (true)
            {
                Console.ResetColor();
                Console.WriteLine("================================================================ nombre de mort: " + NombreMort);
                AfficherMenu(JoueurActuel, ennemiRencontre);
                string input = Console.ReadLine();
                AfficherProfilJoueur();
                if (input.ToLower() == "a" || input.ToLower() == "attaque")
                {
                    JoueurActuel.Attaquer(Ennemis[0]); GestionTourEnnemi();
                }
                else if (input.ToLower() == "d" || input.ToLower() == "défendre")
                {
                    JoueurActuel.Defendre();
                    GestionTourEnnemi();
                }
                /*
                else if (input.ToLower() == "f" || input.ToLower() == "fuir")
                {
                    if (JoueurActuel.Courir())
                    {
                        Console.WriteLine($"{JoueurActuel.Nom} réussit à fuir !");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"{JoueurActuel.Nom} échoue à fuir et doit affronter l'ennemi !");
                        GestionTourEnnemi();
                    }
                }
                */
                else if (input.ToLower() == "s" || input.ToLower() == "soin")
                {
                    JoueurActuel.UtiliserPotion();
                    GestionTourEnnemi();
                }



                else if (input.ToLower() == "av" || input.ToLower() == "avancer")
                {
                    if (Ennemis.Count == 0)
                    {
                        Console.WriteLine("Il n'y a plus d'ennemis à combattre !");
                    }
                    else
                    {
                        RencontrerEnnemi();
                    }
                }
            }
        }

        static void GestionTourEnnemi()
        {
            foreach (var ennemi in Ennemis)
            {
                ennemi.Attaquer(JoueurActuel);
            }
        }

        static void Introduction()
        {
            Console.WriteLine("Le joueur se promène dans un village paisible...");
            Console.WriteLine("Soudain, un ennemi apparaît par surprise! " + Environment.NewLine);
        }

        static void AfficherProfilJoueur()
        {
            Console.WriteLine($"Nom du joueur: {JoueurActuel.Nom}");
            Console.WriteLine($"Niveau: {JoueurActuel.Niveau} | Expérience: {JoueurActuel.Experience}/{JoueurActuel.ExperienceNecessaire}");
            Console.WriteLine($"Vie: {JoueurActuel.Vie} / {JoueurActuel.VieMax}");
            Console.WriteLine("Équipement actuel:");

            foreach (var objet in JoueurActuel.Inventaire)
            {
                Console.WriteLine($" - {objet.Nom}");
            }
        }

        static void AfficherMenu(Joueur joueur, Ennemi ennemi)
        {
            Console.ResetColor();
            Console.WriteLine(Environment.NewLine + "============================================== nombre de mort: " + NombreMort);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("| (A)ttaque | (D)éfendre | (F)uir | (S)oin |");
            Console.ResetColor();
            Console.WriteLine("==============================================");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Profil de {joueur.Nom}: Niveau = {joueur.Niveau}, XP = {joueur.Experience}/{joueur.ExperienceNecessaire}, Vie = {joueur.Vie}, Attaque = {joueur.Degat}");


            Console.WriteLine($"Arme équipée = {joueur.ObtenirArmeEquipee().Nom}, Dégâts de l'arme = {joueur.ObtenirArmeEquipee().Degat}");


            var bouclierEquipe = joueur.Inventaire.FirstOrDefault(o => o is Bouclier) as Bouclier;
            if (bouclierEquipe != null)
            {
                Console.WriteLine($"Bouclier équipé = {bouclierEquipe.Nom}, Protection du bouclier = {bouclierEquipe.Protection}");
            }

            var armureEquipee = joueur.Inventaire.FirstOrDefault(o => o is Armure) as Armure;
            if (armureEquipee != null)
            {
                Console.WriteLine($"Armure équipée = {armureEquipee.Nom}, Protection de l'armure = {armureEquipee.Protection}");
            }

            var potionEquipee = joueur.Inventaire.FirstOrDefault(o => o is Potion) as Potion;
            if (potionEquipee != null)
            {
                Console.WriteLine($"Potion équipée = {potionEquipee.Nom}, Soin de la potion = {potionEquipee.Soin}");
            }
            Console.ResetColor();
            AfficherInfosEnnemi(ennemi);
        }

        static void GenererObjetAleatoire(Rarete rareteEnnemi)
        {
            Random random = new Random();
            int typeObjet = random.Next(4); double tauxDeDropAjuste = ObtenirMultiplicateurTauxDrop(rareteEnnemi);

            if (random.NextDouble() < tauxDeDropAjuste)
            {
                switch (typeObjet)
                {
                    case 0:
                        JoueurActuel.AjouterObjetInventaire(GenererArmeAleatoire(rareteEnnemi));
                        break;
                    case 1:
                        JoueurActuel.AjouterObjetInventaire(GenererArmureAleatoire(rareteEnnemi));
                        break;
                    case 2:
                        JoueurActuel.AjouterObjetInventaire(GenererBouclierAleatoire(rareteEnnemi));
                        break;
                    case 3:
                        JoueurActuel.AjouterObjetInventaire(GenererPotionAleatoire(rareteEnnemi));
                        break;
                }
            }
        }

        static double ObtenirMultiplicateurTauxDrop(Rarete rarete)
        {
            switch (rarete)
            {
                case Rarete.Normal:
                    return 1.0;
                case Rarete.Rare:
                    return 1.5;
                case Rarete.Epic:
                    return 2.0;
                case Rarete.Legendaire:
                    return 4.0;
                case Rarete.Mythique:
                    return 10.0;
                default:
                    return 1.0;
            }
        }

        static void RencontrerEnnemi()
        {

            int indiceEnnemiActuel = 0;
            if (Ennemis.Count > 0)
            {
                Ennemi ennemiRencontre = Ennemis[0];
                Console.Write("Vous rencontrez un ennemi : ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ennemiRencontre.Nom);
                Console.ResetColor();
                AfficherInfosEnnemi(ennemiRencontre);


                while (ennemiRencontre.Vie > 0 && JoueurActuel.Vie > 0)
                {
                    ennemiRencontre.Attaquer(JoueurActuel);

                    if (JoueurActuel.Vie <= 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"{JoueurActuel.Nom}");
                        Console.ResetColor();

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($" a été vaincu par {ennemiRencontre.Nom} !");

                        Console.WriteLine("=============================GAME OVER===========================");

                        Console.WriteLine("Appuyez sur 'r' pour recommencer ou 'q' pour quitter.");

                        Console.WriteLine("=================================================================");
                        Console.ResetColor();
                        string restartInput = Console.ReadLine();

                        if (restartInput.ToLower() == "r")
                        {
                            NombreMort++;
                            JoueurActuel.Vie = JoueurActuel.VieMax;
                            Ennemis = GenererEnnemis();
                            Console.WriteLine("Le jeu a été réinitialisé. Bonne chance !");
                        }
                        else if (restartInput.ToLower() == "q")
                        {
                            Console.WriteLine("Merci d'avoir joué ! À la prochaine.");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Commande non reconnue. Le jeu prend fin.");
                            break;
                        }
                    }


                    AfficherMenu(JoueurActuel, ennemiRencontre);
                    string input = Console.ReadLine();

                    if (input.ToLower() == "a" || input.ToLower() == "attaque")
                    {
                        JoueurActuel.Attaquer(ennemiRencontre);
                        if (ennemiRencontre.Vie <= 0)
                        {
                            Console.WriteLine($"{JoueurActuel.Nom} a vaincu l'ennemi !");
                            GenererObjetAleatoire(ennemiRencontre.Rarete); JoueurActuel.GagnerExperience(ennemiRencontre);
                            Ennemis.RemoveAt(0);

                            if (Ennemis.Count > 0)
                            {

                                ennemiRencontre = Ennemis[0];
                                Console.Write("Vous rencontrez un nouvel ennemi : ");
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(ennemiRencontre.Nom);
                                Console.ResetColor();
                                AfficherInfosEnnemi(ennemiRencontre);

                            }
                            else
                            {
                                Console.WriteLine("Vous avez vaincu tous les ennemis !");
                                break;
                            }
                        }
                    }
                    else if (input.ToLower() == "d" || input.ToLower() == "défendre")
                    {
                        JoueurActuel.Defendre();
                    }
                    /* if (JoueurActuel.Courir())
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{JoueurActuel.Nom} réussit à fuir !");
                        Console.ResetColor();

                                                indiceEnnemiActuel++;

                                                if (indiceEnnemiActuel >= Ennemis.Count)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Vous avez fui avec succès tous les ennemis !");
                            Console.ResetColor();
                            break;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Vous faites face à un nouvel ennemi : {Ennemis[indiceEnnemiActuel].Nom} !");
                            Console.ResetColor();

                                                        Ennemis[indiceEnnemiActuel].Attaquer(JoueurActuel);
                        }
                    }
                    */
                    else if (input.ToLower() == "s" || input.ToLower() == "soin")
                    {
                        JoueurActuel.UtiliserPotion();
                        AfficherInfosEnnemi(ennemiRencontre);
                    }

                }
            }
            else
            {
                Console.WriteLine("Il n'y a plus d'ennemis à combattre !");
            }
        }

        static void AfficherInfosEnnemi(Ennemi ennemi)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Ennemi : {ennemi.Nom} | Vie : {ennemi.Vie}/{ennemi.VieMax} | Dégâts : {ennemi.Degat}");
            Console.ResetColor();
        }
        static List<Ennemi> GenererEnnemis()
        {
            List<Ennemi> ennemis = new List<Ennemi>();

            for (int i = 0; i < 20; i++)
            {
                ennemis.Add(GenererEnnemiAleatoire());
            }

            return ennemis;
        }

        static Ennemi GenererEnnemiAleatoire()
        {
            Random random = new Random();
            Rarete rarete = ChoisirRaretéAjustée(random);

            double multiplicateurVie = ObtenirMultiplicateur(rarete.ToString());
            double multiplicateurDegat = ObtenirMultiplicateur(rarete.ToString());
            double multiplicateurDrop = ObtenirMultiplicateur(rarete.ToString());

            if (rarete == Rarete.Mythique)
            {
                multiplicateurVie = 10;
                multiplicateurDegat = 5;
                multiplicateurDrop = 3;
            }

            string[] nomsCommuns = { "Gobelin", "Squelette", "Loup", "Orc", "Araignée géante" };
            string[] nomsRares = { "Minotaure", "Banshee", "Basilic", "Liche", "Chimère" };
            string[] nomsEpics = { "Dragon", "Kraken", "Phénix", "Hydre", "Griffon" };
            string[] nomsLegendaires = { "Cerbère", "Manticore", "Yeti", "Harpie", "Jinn" };
            string[] nomsMythiques = { "Roi Démon" };

            string[] nomsMonstres;

            switch (rarete)
            {
                case Rarete.Normal:
                    nomsMonstres = nomsCommuns;
                    break;
                case Rarete.Rare:
                    nomsMonstres = nomsRares;
                    break;
                case Rarete.Epic:
                    nomsMonstres = nomsEpics;
                    break;
                case Rarete.Legendaire:
                    nomsMonstres = nomsLegendaires;
                    break;
                case Rarete.Mythique:
                    nomsMonstres = nomsMythiques;
                    break;
                default:
                    nomsMonstres = nomsCommuns;
                    break;
            }

            int indexNom = random.Next(nomsMonstres.Length);

            return new Ennemi($"{nomsMonstres[indexNom]} {rarete}", (int)(random.Next(5, 20) * multiplicateurVie), (int)(random.Next(2, 10) * multiplicateurDegat), rarete, multiplicateurDrop);
        }

        static Rarete ChoisirRaretéAjustée(Random random)
        {
            double probaNormal = 0.4;
            double probaRare = 0.3;
            double probaEpic = 0.2;
            double probaLegendaire = 0.1;

            double randomValue = random.NextDouble();

            if (randomValue < probaNormal)
                return Rarete.Normal;
            else if (randomValue < probaNormal + probaRare)
                return Rarete.Rare;
            else if (randomValue < probaNormal + probaRare + probaEpic)
                return Rarete.Epic;
            else
                return Rarete.Legendaire;
        }

        public static Rarete ConvertToRarete(string rarete)
        {
            switch (rarete.ToLower())
            {
                case "normal":
                    return Rarete.Normal;
                case "rare":
                    return Rarete.Rare;
                case "epic":
                    return Rarete.Epic;
                case "legendaire":
                    return Rarete.Legendaire;
                case "mythique":
                    return Rarete.Mythique;
                default:
                    return Rarete.Normal;
            }
        }

        static Arme GenererArmeAleatoire(Rarete rareteEnnemi)
        {
            Random random = new Random();
            double multiplicateur = ObtenirMultiplicateur(rareteEnnemi.ToString());

            return new Arme($"{rareteEnnemi} {ObtenirNomArme(rareteEnnemi.ToString())}") { Degat = (int)(random.Next(1, 6) * multiplicateur) };
        }

        static string ObtenirNomArme(string rarete)
        {
            Random random = new Random();
            switch (rarete)
            {
                case "Normal":
                    string[] nomsNormaux = { "Épée en bois", "Hache rouillée", "Dague émoussée" };
                    return nomsNormaux[random.Next(nomsNormaux.Length)];

                case "Rare":
                    string[] nomsRares = { "Épée enchantée", "Hache d'argent", "Dague acérée" };
                    return nomsRares[random.Next(nomsRares.Length)];

                case "Epic":
                    string[] nomsEpics = { "Épée de flammes", "Hache de glace", "Dague empoisonnée" };
                    return nomsEpics[random.Next(nomsEpics.Length)];

                case "Légendaire":
                    string[] nomsLegendaires = { "Épée du destin", "Hache céleste", "Dague abyssale" };
                    return nomsLegendaires[random.Next(nomsLegendaires.Length)];

                default:
                    return "Arme inconnue";
            }
        }

        static Armure GenererArmureAleatoire(Rarete rareteEnnemi)
        {
            Random random = new Random();
            double multiplicateur = ObtenirMultiplicateur(rareteEnnemi.ToString());

            return new Armure($"{rareteEnnemi} {ObtenirNomArmure(rareteEnnemi.ToString())}") { Protection = (int)(random.Next(1, 6) * multiplicateur) };
        }

        static string ObtenirNomArmure(string rarete)
        {
            Random random = new Random();
            switch (rarete)
            {
                case "Normal":
                    string[] nomsNormaux = { "Cuir léger", "Plaques de fer", "Robe enchantée" };
                    return nomsNormaux[random.Next(nomsNormaux.Length)];

                case "Rare":
                    string[] nomsRares = { "Cuir renforcé", "Armure d'acier", "Robe mystique" };
                    return nomsRares[random.Next(nomsRares.Length)];

                case "Epic":
                    string[] nomsEpics = { "Cuir draconique", "Armure élémentaire", "Robe astrale" };
                    return nomsEpics[random.Next(nomsEpics.Length)];

                case "Légendaire":
                    string[] nomsLegendaires = { "Armure divine", "Armure ancienne", "Robe éthérée" };
                    return nomsLegendaires[random.Next(nomsLegendaires.Length)];

                default:
                    return "Armure inconnue";
            }
        }

        static Bouclier GenererBouclierAleatoire(Rarete rareteEnnemi)
        {
            Random random = new Random();
            double multiplicateur = ObtenirMultiplicateur(rareteEnnemi.ToString());

            return new Bouclier($"{rareteEnnemi} {ObtenirNomBouclier(rareteEnnemi.ToString())}") { Protection = (int)(random.Next(1, 6) * multiplicateur) };
        }

        static string ObtenirNomBouclier(string rarete)
        {
            Random random = new Random();
            switch (rarete)
            {
                case "Normal":
                    string[] nomsNormaux = { "Bouclier en bois", "Bouclier de fer", "Bouclier de cuir" };
                    return nomsNormaux[random.Next(nomsNormaux.Length)];

                case "Rare":
                    string[] nomsRares = { "Bouclier enchanté", "Bouclier d'argent", "Bouclier renforcé" };
                    return nomsRares[random.Next(nomsRares.Length)];

                case "Epic":
                    string[] nomsEpics = { "Bouclier flamboyant", "Bouclier glacial", "Bouclier éthéré" };
                    return nomsEpics[random.Next(nomsEpics.Length)];

                case "Légendaire":
                    string[] nomsLegendaires = { "Bouclier divin", "Bouclier ancestral", "Bouclier cosmique" };
                    return nomsLegendaires[random.Next(nomsLegendaires.Length)];

                default:
                    return "Bouclier inconnu";
            }
        }

        static Potion GenererPotionAleatoire(Rarete rareteEnnemi)
        {
            Random random = new Random();
            double multiplicateur = ObtenirMultiplicateur(rareteEnnemi.ToString());

            return new Potion($"{rareteEnnemi} {ObtenirNomPotion(rareteEnnemi.ToString())}") { Soin = (int)(random.Next(5, 11) * multiplicateur) };
        }

        static string ObtenirNomPotion(string rarete)
        {
            Random random = new Random();
            switch (rarete)
            {
                case "Normal":
                    string[] nomsNormaux = { "Potion de soin légère", "Élixir de vitalité", "Potion de guérison mineure" };
                    return nomsNormaux[random.Next(nomsNormaux.Length)];

                case "Rare":
                    string[] nomsRares = { "Potion de soin avancée", "Élixir de régénération", "Potion de guérison majeure" };
                    return nomsRares[random.Next(nomsRares.Length)];

                case "Epic":
                    string[] nomsEpics = { "Potion de soin éthérée", "Élixir de résurrection", "Potion de guérison suprême" };
                    return nomsEpics[random.Next(nomsEpics.Length)];

                case "Légendaire":
                    string[] nomsLegendaires = { "Potion de soin divine", "Élixir d'immortalité", "Potion de guérison ultime" };
                    return nomsLegendaires[random.Next(nomsLegendaires.Length)];

                default:
                    return "Potion inconnue";
            }
        }

        static string ChoisirRareté(Random random)
        {
            double[] probabilites = { 0.6, 0.3, 0.08, 0.02 };
            double randomValue = random.NextDouble();

            if (randomValue < probabilites[0]) return "Normal";
            else if (randomValue < probabilites[1]) return "Rare";
            else if (randomValue < probabilites[2]) return "Epic";
            else return "Légendaire";
        }

        static double ObtenirMultiplicateur(string rarete)
        {
            switch (rarete)
            {
                case "Normal": return 1.0;
                case "Rare": return 2.0;
                case "Epic": return 4.0;
                case "Légendaire": return 8.0;
                default: return 1.0;
            }
        }
    }

    

    public enum Rarete
    {
        Normal,
        Rare,
        Epic,
        Legendaire,
        Mythique
    }

    

}
