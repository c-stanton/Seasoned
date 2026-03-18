import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import RecipeDisplay from '../app/components/RecipeDisplay.vue'

const vuetify = createVuetify({
  components,
  directives,
})

describe('RecipeDisplay.vue', () => {
  const mockRecipe = {
    title: 'Bakery-Style Muffins',
    description: 'Fresh from the oven.',
    ingredients: ['2 cups flour', '1 cup sugar'],
    instructions: ['Preheat oven', 'Bake muffins'],
    imageUrl: 'data:image/png;base64,header_captured_image'
  }

  it('renders the title and all ingredients correctly', () => {
    const wrapper = mount(RecipeDisplay, {
      props: { recipe: mockRecipe },
      global: { plugins: [vuetify] }
    })

    expect(wrapper.find('.recipe-title').text()).toBe('Bakery-Style Muffins')
    
    const ingredientItems = wrapper.findAll('.ingredient-item')
    expect(ingredientItems).toHaveLength(2)
    expect(ingredientItems[0].text()).toContain('2 cups flour')
  })

  it('displays the recipe image when imageUrl is provided', () => {
    const wrapper = mount(RecipeDisplay, {
      props: { recipe: mockRecipe },
      global: { plugins: [vuetify] }
    })
    
    const img = wrapper.findComponent({ name: 'VImg' })
    expect(img.exists()).toBe(true)
    expect(img.props('src')).toBe(mockRecipe.imageUrl)
  })

  it('emits "save" when the save button is clicked', async () => {
    const wrapper = mount(RecipeDisplay, {
      props: { 
        recipe: mockRecipe,
        isSaving: false,
        hasSaved: false 
      },
      global: {
        plugins: [vuetify]
      }
    })

    const saveBtn = wrapper.find('.save-recipe-btn')
    await saveBtn.trigger('click')

    expect(wrapper.emitted()).toHaveProperty('save')
  })

  it('shows the "Saved in Archives" state when hasSaved is true', async () => {
    const recipe = { title: 'Bakery-Style Muffins', ingredients: [], instructions: [] }
    const wrapper = mount(RecipeDisplay, {
      props: {
        recipe,
        hasSaved: true
      },
      global: { plugins: [vuetify] }
    })

    expect(wrapper.text()).toContain('Saved in Archives')
  })

  it('triggers the browser print dialog when the print button is clicked', async () => {
    const printSpy = vi.spyOn(window, 'print').mockImplementation(() => {})
    
    const wrapper = mount(RecipeDisplay, {
      props: { recipe: mockRecipe },
      global: { plugins: [vuetify] }
    })

    const printBtn = wrapper.find('.print-btn')
    await printBtn.trigger('click')

    expect(printSpy).toHaveBeenCalled()
  })
})