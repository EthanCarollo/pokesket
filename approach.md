# Approche de Développement du Projet

## Concept de Base
Nous avons développé un jeu original et fun en mélangeant **l’univers du basket et des Pokémon** dans des matchs **3V3 en local**, dans un style arcade inspiré des modes "basket street" de NBA 2K.

> Pokésket (oui Pokémon Basket)

L’objectif : créer **un petit jeu compétitif, rapide et accessible entre amis.** (On en avait marre de jouer à Mario Tennis)

## Inspirations
- **NBA 2K (mode street/basket de rue)**  
  → Système de gameplay, style 3V3, feeling des tirs et des déplacements, avec une approche arcade. 
  
- **Super Smash Bros.**  
  → Inspiration pour la **sélection de personnages dynamique et visuelle**, avec une interface intuitive qui encourage des choix rapides.

- **Dunk City Dynasty**  
  → Influence sur **les contrôles fluides et nerveux**, pour apporter un gameplay arcade simple et réactif.

- **Mario Strikers**  
  → Référence pour le **concept de mélanger une licence culte avec un sport**, en assumant une ambiance fun, décalée et spectaculaire.

## Recherche et Intégration des Assets
- Assets trouvés principalement sur :
  - **Itch.io** (éléments graphiques libres ou adaptés pour Unity)
  - **The Spriters Resource** (sprites et ressources issus de Pokémon)
  - **Game UI Database** (Inspirations et autres assets)
  
- Nous avons intégré **un maximum d'éléments visuels et sonores des Pokémon** pour renforcer l’identité du jeu.

- Notre objectif était de **rendre le prototype riche et vivant**, en ajoutant de nombreux détails pour une expérience déjà complète dès cette première version.

## Logiciels Utilisés
- **Unity** pour la création du jeu vidéo.
- **Photoshop** pour le redesign des assets récupérés sur les sites cités précédemment.
- **Aseprite** pour la création d'UI avec un aspect pixelisé.
- Tentative d'utilisation de **FairyGUI**, qui théoriquement permet de designer un canvas plus facilement et importable directement dans Unity.

## Construction du Prototype
- Création d'un **prototype jouable et fonctionnel en local 3V3.**
- Principaux éléments développés :
  - **Gameplay arcade inspiré de NBA 2K street et Dunk City.**
  - **Sélection des personnages type Smash Bros.**
  - **Ambiance fun et compétitive**, proche de Mario Strikers.

- Le prototype offre une base solide avec des fonctionnalités clés pour pouvoir itérer par la suite.

## Idées de Fonctionnalités Futures
Voici une sélection de fonctionnalités non prioritaires mais intéressantes que nous avons envisagées pour prolonger l’expérience :

- **Faire évoluer les personnages** au cours des matchs ou sur plusieurs parties.
- **Ajouter une animation de danse pour le MVP** en fin de match.
- **Repositionner les personnages** automatiquement en début d’action ou après certains événements.
- **Juice et feedback visuel/textuel lors du scoring**, plus satisfaisant et expressif.
- **Ajout de QTE** pour ajouter du skill pour les dunk.
- **Différents modes de jeu** pour ajouter de la diversité et de la rejouabilité.
- **Pouvoir jouer seul ou jusqu'à 4 joueurs** pour ajouter du challenge entre amis et la possibilité de s'entraîner avant.
- **Ajouter des versions différentes de pokémon**, qui posséderaient des statistiques différentes.
- **Faire fonctionner correctement la version web**, afin de faire profiter les gens du monde entier.
- **Pouvoir modifier le nombre de points à gagner pour remporter le match**, permet la personnalisation indirect du temps de jeu.
- **Mettre l'entièreté du pokédex dans le jeu**, pour faire plaisir aux fans de Pokémon.
- **Ajout de particules quand un pokémon court ou qu'il fait un dash défensif** pour encore plus de Juice et de satisfaction visuel.

Ces idées restent ouvertes pour de futures itérations et seraient de belles opportunités d'amélioration du projet si on poursuit le développement.

## Approche Technique avec Unity
En tant que développeurs, nous avons pris le temps de structurer notre projet Unity de manière claire et modulaire :

- **Nous avons également adopté une approche orientée `ScriptableObject` pour mieux organiser les données de gameplay**, en séparant clairement la logique du code et les contenus comme les types de Pokémon, leurs statistiques, ou encore la base de données utilisée pour la sélection des personnages.
- Utilisation d’un **système d’animation basé sur les directions**, permettant de déclencher différentes animations de shoot selon l’angle d’attaque du joueur.
- Création d’un **système de shoot dynamique**, avec barre de timing, calculs physiques, et ajustement automatique de la trajectoire vers l’arceau.
- Organisation rigoureuse avec **des composants séparés par rôle** (gestion de l’entrée, animation, physique du ballon, feedback UI...).
- Gestion du changement de scène avec une **instance centralisée de `SceneTransitor`**, évitant les références rigides à des GameObjects.
- Mise en place d’un système de **detection de zone 2 pts / 3 pts** pour un scoring plus stratégique.
- Utilisation du **`RequireComponent`** pour assurer la robustesse des composants critiques comme les contrôles ou la physique.
- Tests réguliers sur différents supports (WebGL, desktop) pour garantir la stabilité du gameplay.
- Et bien sûr… quelques `Debug.Log("c'est censé marcher là")` stratégiques par ci par là.

On a pris du plaisir à réfléchir à des solutions efficaces, à se challenger pour rendre le jeu stable et à tirer parti de tout ce que Unity pouvait nous offrir en peu de temps.

## Processus de Conception Ludique et Flexible
- Nous avons souhaité **nous amuser pendant la conception** :  
  - Jamais fermés à une nouvelle idée, on a toujours dit "oui" aux propositions (oui vraiment, sauf à la fin, c'était chaud un peu).  
  - Le processus a été un **cycle constant : idée → concrétisation → test → nouvelle idée**, pour qu'on puisse passer à autre chose et avoir un rendu présentable.

- Grâce à cette méthode de production, nous n'avons jamais arrêté de communiquer, ce qui a grandement augmenté notre productivité. On est toujours resté motivé par l'idée de créer un jeu, et la concrétisation rapide de nos idées nous tenait en haleine.

Bref, en somme on a vraiment kiffé produire ce projet.
