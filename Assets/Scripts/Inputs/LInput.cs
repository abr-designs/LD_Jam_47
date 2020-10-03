public static class LInput
{
   public static Game Input => _gameInput ?? (_gameInput = new Game());
   private static Game _gameInput;
}
