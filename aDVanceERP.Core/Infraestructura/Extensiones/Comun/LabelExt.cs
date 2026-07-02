namespace aDVanceERP.Core.Infraestructura.Extensiones.Comun {
    public static class LabelExt {
        public static void AjusteAutomaticoMargenTexto(this Label label) {
            var dimensionesTexto = TextRenderer.MeasureText(label.Text, label.Font);

            label.Margin = new Padding(1, dimensionesTexto.Width > label.Width ? 8 : 1, 1, 1);
        }
    }
}
