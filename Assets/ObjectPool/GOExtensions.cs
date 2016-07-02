namespace Mandarin {
    public static class GOPoolExtensions {

        public static GOPool Pool(this GO go, int num) {
            return GOPool.Create(num)
                .Fill(i => {
                    return go.Duplicate().Modify()
                        // set active
                        .SetName(go.gameObject.name + "_" + i)
                        .SetParent(go.transform.parent)
                        .transform;
                });
        }
    }
}
