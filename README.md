# 🚀 Speedtest Iperf

**Speedtest Iperf** est une application Windows moderne et légère conçue pour mesurer les débits réseau (Upload et Download) en s'appuyant sur le moteur robuste **Iperf3**. 

L'outil offre une interface fluide et facile à prendre en main, tout en permettant l'exportation des résultats pour un suivi technique précis.

---

## ✨ Fonctionnalités

* **Mesure Double Flux** : Tests séquentiels du débit montant (Upload) et descendant (Download).
* **Console Temps Réel** : Suivi détaillé de l'exécution d'Iperf3 directement dans l'application.
* **Export CSV** : Sauvegarde des résultats (Date, Heure, Mbps) dans un fichier nouveau ou existant.
* **Installation Flexible** : Installateur "Dual-Mode" (installation avec ou sans droits d'administrateur).

---

## 📸 Aperçu

> *[Ajoute d'une capture d'écran de l'application]*

---

## 🛠️ Installation

1. Téléchargez la dernière version du setup dans l'onglet [Releases](https://github.com/yatsuuw/IperfApp/releases).
2. Lancez `Speedtest_Iperf_v1.0.1_Setup.exe`.
3. Choisissez l'installation pour "Moi uniquement" (sans admin) ou "Pour tous les utilisateurs".

---

## 💻 Pour les Développeurs

Si vous souhaitez compiler le projet vous-même :

### Prérequis
* [.NET 10 SDK](https://dotnet.microsoft.com/fr-fr/download)
* Visual Studio 2022 ou VS Code

### Compilation
Pour générer l'exécutable léger (Framework-Dependent) :
```bash
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
