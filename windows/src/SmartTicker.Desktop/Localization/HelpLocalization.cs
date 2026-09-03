using System;
using System.Collections.Generic;
using SmartTicker.Core.Models;

namespace SmartTicker.Desktop.Localization;

public sealed record HelpStrings(
    string Title,
    string Subtitle,
    string Navigation,
    string CheckingOnline,
    string OnlineLoaded,
    string OfflineLoaded);

public static class HelpLocalization
{
    private static readonly IReadOnlyDictionary<string, HelpStrings> Map =
        new Dictionary<string, HelpStrings>(StringComparer.OrdinalIgnoreCase)
        {
            ["en"] = new("SmartTicker Help", "Configuration guide", "ON THIS PAGE", "Built-in guide loaded. Checking for online updates...", "Online guide loaded from the SmartTicker repository.", "Online help is unavailable. Showing the built-in guide."),
            ["ar"] = new("مساعدة SmartTicker", "دليل الإعداد", "في هذه الصفحة", "تم تحميل الدليل المدمج. جارٍ البحث عن تحديثات عبر الإنترنت...", "تم تحميل الدليل عبر الإنترنت من مستودع SmartTicker.", "المساعدة عبر الإنترنت غير متاحة. يتم عرض الدليل المدمج."),
            ["de"] = new("SmartTicker-Hilfe", "Konfigurationshandbuch", "AUF DIESER SEITE", "Integriertes Handbuch geladen. Online-Updates werden gesucht...", "Online-Handbuch aus dem SmartTicker-Repository geladen.", "Online-Hilfe ist nicht verfügbar. Das integrierte Handbuch wird angezeigt."),
            ["el"] = new("Βοήθεια SmartTicker", "Οδηγός ρύθμισης", "ΣΕ ΑΥΤΗ ΤΗ ΣΕΛΙΔΑ", "Ο ενσωματωμένος οδηγός φορτώθηκε. Έλεγχος για ενημερώσεις...", "Ο ηλεκτρονικός οδηγός φορτώθηκε από το αποθετήριο SmartTicker.", "Η ηλεκτρονική βοήθεια δεν είναι διαθέσιμη. Εμφανίζεται ο ενσωματωμένος οδηγός."),
            ["es"] = new("Ayuda de SmartTicker", "Guía de configuración", "EN ESTA PÁGINA", "Guía integrada cargada. Buscando actualizaciones en línea...", "Guía en línea cargada desde el repositorio de SmartTicker.", "La ayuda en línea no está disponible. Se muestra la guía integrada."),
            ["fr"] = new("Aide SmartTicker", "Guide de configuration", "SUR CETTE PAGE", "Guide intégré chargé. Recherche des mises à jour en ligne...", "Guide en ligne chargé depuis le dépôt SmartTicker.", "L'aide en ligne est indisponible. Affichage du guide intégré."),
            ["hi"] = new("SmartTicker सहायता", "कॉन्फ़िगरेशन मार्गदर्शिका", "इस पृष्ठ पर", "अंतर्निहित मार्गदर्शिका लोड हुई। ऑनलाइन अपडेट जाँचे जा रहे हैं...", "SmartTicker रिपॉज़िटरी से ऑनलाइन मार्गदर्शिका लोड हुई।", "ऑनलाइन सहायता उपलब्ध नहीं है। अंतर्निहित मार्गदर्शिका दिखाई जा रही है।"),
            ["id"] = new("Bantuan SmartTicker", "Panduan konfigurasi", "DI HALAMAN INI", "Panduan bawaan dimuat. Memeriksa pembaruan daring...", "Panduan daring dimuat dari repositori SmartTicker.", "Bantuan daring tidak tersedia. Menampilkan panduan bawaan."),
            ["it"] = new("Guida di SmartTicker", "Guida alla configurazione", "IN QUESTA PAGINA", "Guida integrata caricata. Ricerca di aggiornamenti online...", "Guida online caricata dal repository SmartTicker.", "La guida online non è disponibile. Viene mostrata la guida integrata."),
            ["ja"] = new("SmartTicker ヘルプ", "設定ガイド", "このページの内容", "内蔵ガイドを読み込みました。オンライン更新を確認しています...", "SmartTicker リポジトリからオンラインガイドを読み込みました。", "オンラインヘルプを利用できません。内蔵ガイドを表示しています。"),
            ["ko"] = new("SmartTicker 도움말", "설정 가이드", "이 페이지의 내용", "내장 가이드를 불러왔습니다. 온라인 업데이트를 확인하는 중...", "SmartTicker 저장소에서 온라인 가이드를 불러왔습니다.", "온라인 도움말을 사용할 수 없습니다. 내장 가이드를 표시합니다."),
            ["nl"] = new("SmartTicker-help", "Configuratiehandleiding", "OP DEZE PAGINA", "Ingebouwde handleiding geladen. Online-updates worden gecontroleerd...", "Onlinehandleiding geladen uit de SmartTicker-repository.", "Onlinehelp is niet beschikbaar. De ingebouwde handleiding wordt getoond."),
            ["pt"] = new("Ajuda do SmartTicker", "Guia de configuração", "NESTA PÁGINA", "Guia integrado carregado. Verificando atualizações online...", "Guia online carregado do repositório SmartTicker.", "A ajuda online não está disponível. Exibindo o guia integrado."),
            ["ru"] = new("Справка SmartTicker", "Руководство по настройке", "НА ЭТОЙ СТРАНИЦЕ", "Встроенное руководство загружено. Проверка обновлений в сети...", "Онлайн-руководство загружено из репозитория SmartTicker.", "Онлайн-справка недоступна. Показано встроенное руководство."),
            ["tr"] = new("SmartTicker Yardımı", "Yapılandırma kılavuzu", "BU SAYFADA", "Yerleşik kılavuz yüklendi. Çevrimiçi güncellemeler denetleniyor...", "Çevrimiçi kılavuz SmartTicker deposundan yüklendi.", "Çevrimiçi yardım kullanılamıyor. Yerleşik kılavuz gösteriliyor."),
            ["zh"] = new("SmartTicker 帮助", "配置指南", "本页内容", "已加载内置指南。正在检查在线更新...", "已从 SmartTicker 仓库加载在线指南。", "在线帮助不可用。正在显示内置指南。"),
        };

    public static HelpStrings For(string? language)
    {
        var code = AppLanguages.Normalize(language);
        return Map.TryGetValue(code, out var strings) ? strings : Map[AppLanguages.Default];
    }
}
