namespace TCDefectIntegration {
    public static class IntegratorFactory {
        public static Integrator GetIntegrator() {
            return new JiraIntegrator();
        }
    }
}