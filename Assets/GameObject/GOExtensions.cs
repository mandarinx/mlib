namespace Mlib {
    public static class GOPoolExtensions {

        public static GOPool Pool(this GO go, int num) {
            return GOPool.Create(num)
                .Fill(i => go.Duplicate().Modify()
                              // set active
                             .SetName(go.GameObject.name + "_" + i)
                             .SetParent(go.Transform.parent)
                             .Transform);
        }
    }
}
