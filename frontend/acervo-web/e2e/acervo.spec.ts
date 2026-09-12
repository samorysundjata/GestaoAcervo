import { test, expect, type Page } from '@playwright/test';

function uniqueSuffix(): string {
  return Date.now().toString(36);
}

async function fillMatInput(page: Page, label: string, value: string): Promise<void> {
  await page.getByLabel(label).click();
  await page.getByLabel(label).fill(value);
}

async function saveAndReloadList(
  page: Page,
  successText: string,
  listPath: string,
  heading: string,
): Promise<void> {
  await page.getByRole('button', { name: 'Salvar' }).click();
  await expect(page.getByText(successText)).toBeVisible();
  await expect(page.getByRole('heading', { name: heading })).toBeVisible();
  await page.goto(listPath);
  await expect(page.getByRole('heading', { name: heading })).toBeVisible();
}

test.describe('Acervo SPA', () => {
  test('default route lists books', async ({ page }) => {
    await page.goto('/');
    await expect(page.getByRole('heading', { name: 'Livros' })).toBeVisible();
    await expect(page.getByRole('link', { name: /Novo Livro/ })).toBeVisible();
  });

  test('genre CRUD', async ({ page }) => {
    const nome = `Genero ${uniqueSuffix()}`;
    const renamed = `${nome} edit`;

    await page.goto('/generos');
    await page.getByRole('link', { name: /Novo Gênero/ }).click();
    await fillMatInput(page, 'Nome', nome);
    await saveAndReloadList(page, 'Gênero criado com sucesso!', '/generos', 'Gêneros');
    await expect(page.getByRole('cell', { name: nome })).toBeVisible();

    const row = page.getByRole('row', { name: new RegExp(nome) });
    await row.getByRole('link', { name: 'Editar' }).click();
    await fillMatInput(page, 'Nome', renamed);
    await saveAndReloadList(page, 'Gênero atualizado com sucesso!', '/generos', 'Gêneros');
    await expect(page.getByRole('cell', { name: renamed })).toBeVisible();

    await page.getByRole('row', { name: new RegExp(renamed) }).getByRole('button', { name: 'Excluir' }).click();
    await page.getByRole('button', { name: 'Confirmar' }).click();
    await expect(page.getByText('Gênero excluído com sucesso!')).toBeVisible();
    await expect(page.getByRole('cell', { name: renamed })).toHaveCount(0);
  });

  test('create author and book, then block deleting the linked author', async ({ page }) => {
    const suffix = uniqueSuffix();
    const autorNome = `Autor ${suffix}`;
    const generoNome = `GenLivro ${suffix}`;
    const titulo = `Livro ${suffix}`;
    const isbn = String(Date.now()).slice(-13).padStart(13, '9');

    await page.goto('/autores');
    await page.getByRole('link', { name: /Novo Autor/ }).click();
    await fillMatInput(page, 'Nome', autorNome);
    await fillMatInput(page, 'E-mail', `autor.${suffix}@test.com`);
    await saveAndReloadList(page, 'Autor criado com sucesso!', '/autores', 'Autores');
    await expect(page.getByRole('cell', { name: autorNome })).toBeVisible();

    await page.getByRole('link', { name: 'Gêneros' }).click();
    await page.getByRole('link', { name: /Novo Gênero/ }).click();
    await fillMatInput(page, 'Nome', generoNome);
    await saveAndReloadList(page, 'Gênero criado com sucesso!', '/generos', 'Gêneros');
    await expect(page.getByRole('cell', { name: generoNome })).toBeVisible();

    await page.getByRole('link', { name: 'Livros' }).click();
    await page.getByRole('link', { name: /Novo Livro/ }).click();
    await fillMatInput(page, 'Título', titulo);
    await fillMatInput(page, 'ISBN', isbn);
    await fillMatInput(page, 'Ano de Publicação', '1934');
    await page.getByLabel('Autor').click();
    await page.getByRole('option', { name: autorNome }).click();
    await page.getByLabel('Gênero').click();
    await page.getByRole('option', { name: generoNome }).click();
    await saveAndReloadList(page, 'Livro criado com sucesso!', '/livros', 'Livros');
    await expect(page.getByRole('cell', { name: titulo })).toBeVisible();

    await page.getByRole('link', { name: 'Autores' }).click();
    await page.getByRole('row', { name: new RegExp(autorNome) }).getByRole('button', { name: 'Excluir' }).click();
    await page.getByRole('button', { name: 'Confirmar' }).click();
    await expect(page.getByText(/Não é possível excluir um autor com livros vinculados/)).toBeVisible();
    await expect(page.getByRole('cell', { name: autorNome })).toBeVisible();
  });
});
